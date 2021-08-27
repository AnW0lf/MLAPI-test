using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using System.Linq;
using Assets.Scripts.Player;
using System;
using Assets.Scripts.NPC;

namespace Game
{
    public class GameController : NetworkBehaviour
    {
        public static GameController Singleton { get; private set; } = null;

        [Header("Population")]
        [SerializeField] private GameObject[] _networkNpcPrefabs = null;
        [SerializeField] private int _maxCount = 100;
        [SerializeField] private Rect _populationArea = Rect.zero;
        [SerializeField] private float _raycastHeight = 20f;

        public event Action OnBodyEnabled;
        public event Action OnBodyDisabled;

        private readonly NetworkVariableULong _civilianNextId = new NetworkVariableULong(new NetworkVariableSettings
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.ServerOnly
        }, 1);

        private readonly Dictionary<ulong, bool> _connectedClients = new Dictionary<ulong, bool>();

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

            GameObject prefab = _networkNpcPrefabs[Random.Range(0, _networkNpcPrefabs.Length)];
            Vector3 position = RandomPointOnGround;
            Quaternion rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));

            var netNpc = Instantiate(prefab).GetComponent<NetworkNPC>();
            netNpc.GetComponent<NetworkObject>().Spawn();
            netNpc.NpcId = _civilianNextId.Value;
            netNpc.SpawnLocal(position, rotation);

            _civilianNextId.Value++;
        }

        public Vector3 RandomPointOnGround
        {
            get
            {
                Vector3? point;
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

        [ClientRpc]
        private void EnableBodiesClientRpc()
        {
            var localClient = FindObjectsOfType<NetworkLocalPlayer>()
                .First((cc) => cc.OwnerClientId == NetworkManager.Singleton.LocalClientId);

            if (localClient == null) { return; }

            localClient.GetComponent<NetworkLocalPlayer>()
                .SpawnLocal(RandomPointOnGround, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));

            OnBodyEnabled?.Invoke();
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
                for (int i = 0; i < _maxCount - NetworkManager.Singleton.ConnectedClients.Count; i++)
                {
                    SpawnNPC();
                }

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
