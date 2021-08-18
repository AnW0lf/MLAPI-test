using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using System.Linq;
using Assets.Scripts.Player;
using System;

namespace Game
{
    public class GameController : NetworkBehaviour
    {
        public static GameController Singleton { get; private set; } = null;

        [Header("Population")]
        [SerializeField] private GameObject[] _civilianPrefabs = null;
        [SerializeField] private int _maxCount = 100;
        [SerializeField] private Rect _populationArea = Rect.zero;
        [SerializeField] private float _raycastHeight = 20f;

        public event Action OnBodiesEnabled;
        public event Action OnBodiesDisabled;

        private NetworkVariableULong _civilianNextId = new NetworkVariableULong(new NetworkVariableSettings
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.ServerOnly
        }, 1);

        private Dictionary<ulong, bool> _connectedClients = new Dictionary<ulong, bool>();

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(Singleton);
        }

        public override void NetworkStart()
        {
            if (NetworkManager.Singleton == null) { return; }
            if (NetworkManager.Singleton.IsServer)
            {
                for (int i = 0; i < _maxCount - NetworkManager.Singleton.ConnectedClients.Count; i++)
                {
                    SpawnNPC();
                }

                foreach (var pair in NetworkManager.Singleton.ConnectedClients)
                {
                    if (_connectedClients.ContainsKey(pair.Key) == false)
                    {
                        _connectedClients.Add(pair.Key, false);
                    }
                }
            }

            ClientConnectedServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        private void SpawnNPC()
        {
            if (IsServer == false) { return; }

            GameObject prefab = _civilianPrefabs[Random.Range(0, _civilianPrefabs.Length)];
            Vector3 position = RandomPointOnGround;
            Quaternion rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
            var civilian = Instantiate(prefab, position, rotation).GetComponent<Civilian>();
            civilian.GetComponent<NetworkObject>().Spawn();
            civilian.UniqueId = _civilianNextId.Value;
            _civilianNextId.Value++;
        }

        public Vector3 RandomPointOnGround
        {
            get
            {
                Vector3? point = null;
                do
                {
                    point = GetRandomPointOnGround();
                } while (point == null);
                return (Vector3)point;
            }
        }

        private Vector3? GetRandomPointOnGround()
        {
            Ray ray = new Ray(RandomPoint, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastHeight))
            {
                if (hit.transform.CompareTag("Ground"))
                {
                    return hit.point;
                }
            }
            return null;
        }

        private Vector3 RandomPoint
        {
            get
            {
                float x = transform.position.x + Random.Range(-_populationArea.width / 2f, _populationArea.width / 2f);
                float y = transform.position.y + _raycastHeight;
                float z = transform.position.z + Random.Range(-_populationArea.height / 2f, _populationArea.height / 2f);
                return new Vector3(x, y, z);
            }
        }

        private void EnableBodies()
        {
            var players = FindObjectsOfType<NetworkLocalPlayer>();
            foreach (var player in players)
            {
                player.EnableBody(RandomPointOnGround, Quaternion.identity);
            }

            OnBodiesEnabled?.Invoke();
        }

        [ServerRpc(RequireOwnership = true)]
        public void EnableBodiesServerRpc()
        {
            EnableBodiesClientRpc();
        }

        [ClientRpc]
        private void EnableBodiesClientRpc()
        {
            EnableBodies();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClientConnectedServerRpc(ulong clientId)
        {
            if (_connectedClients.ContainsKey(clientId))
            {
                _connectedClients[clientId] = true;
            }

            if (_connectedClients.All((pair) => pair.Value))
            {
                EnableBodiesClientRpc();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3(_populationArea.x, _raycastHeight / 2f, _populationArea.y) + transform.position;
            Vector3 size = new Vector3(_populationArea.width, _raycastHeight, _populationArea.height);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
