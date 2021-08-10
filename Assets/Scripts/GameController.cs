using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

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

        private NetworkVariableULong _civilianNextId = new NetworkVariableULong(new NetworkVariableSettings
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.ServerOnly
        }, 1);

        private Dictionary<ulong, bool> _playerConnected = new Dictionary<ulong, bool>();
        private Player.NetworkPlayer _ownerNetPlayer = null;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(Singleton);
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {

            }
        }

        public override void NetworkStart()
        {
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    foreach (var client in NetworkManager.Singleton.ConnectedClients)
                    {
                        _playerConnected.Add(client.Key, false);
                    }

                    for (int i = 0; i < _maxCount - NetworkManager.Singleton.ConnectedClients.Count; i++)
                    {
                        SpawnNPC();
                    }
                }

                _ownerNetPlayer = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId]
                    .PlayerObject.GetComponent<Player.NetworkPlayer>();
                PlayerConnectedServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }

        private void SpawnNPC()
        {
            if(IsServer == false) { return; }

            Vector3? randomPlace;
            do
            {
                randomPlace = GetRandomPointOnGround();
            } while (randomPlace == null);

            GameObject prefab = _civilianPrefabs[Random.Range(0, _civilianPrefabs.Length)];
            Vector3 position = (Vector3)randomPlace;
            Quaternion rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
            var civilian = Instantiate(prefab, position, rotation).GetComponent<Civilian>();
            civilian.GetComponent<NetworkObject>().Spawn();
            civilian.UniqueId = _civilianNextId.Value;
            _civilianNextId.Value++;
        }

        private void ActivatePlayers()
        {
            foreach (var clientId in _playerConnected.Keys)
            {
                Vector3? randomPlace;
                do
                {
                    randomPlace = GetRandomPointOnGround();
                } while (randomPlace == null);

                ActiveBodyClientRpc(clientId, (Vector3)randomPlace);
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
        private void ActiveBodyClientRpc(ulong clientId, Vector3 position)
        {
            if(clientId != _ownerNetPlayer.OwnerClientId) { return; }

            _ownerNetPlayer.transform.position = position;
            _ownerNetPlayer.IsBodyActive = true;
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerConnectedServerRpc(ulong clientId)
        {
            if (_playerConnected.ContainsKey(clientId))
            {
                _playerConnected[clientId] = true;
                PlayerConnectedClientRpc(clientId);
            }

            if (_playerConnected.ContainsValue(false) == false)
            {
                ActivatePlayers();
            }
        }

        [ClientRpc]
        private void PlayerConnectedClientRpc(ulong clientId)
        {
            if (_playerConnected.ContainsKey(clientId) == false)
            {
                _playerConnected.Add(clientId, false);
            }
            _playerConnected[clientId] = true;
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
