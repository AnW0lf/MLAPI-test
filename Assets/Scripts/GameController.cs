using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
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

        private NetworkVariableULong _civilianNextId = new NetworkVariableULong(new NetworkVariableSettings
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.ServerOnly
        }, 1);

        private Dictionary<ulong, ulong> _playerCivilian = new Dictionary<ulong, ulong>();
        private Dictionary<ulong, bool> _playerConnected = new Dictionary<ulong, bool>();
        private Civilian[] _civilians = null;
        private Player.NetworkPlayer _ownerNetPlayer = null;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(Singleton);
        }

        public override void NetworkStart()
        {
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    foreach (var client in NetworkManager.Singleton.ConnectedClients)
                    {
                        if (!_playerCivilian.ContainsKey(client.Key))
                        {
                            _playerCivilian.Add(client.Key, 0);
                            _playerConnected.Add(client.Key, false);
                        }
                        if (client.Value.PlayerObject.IsOwner)
                        {
                            _ownerNetPlayer = client.Value.PlayerObject.GetComponent<Player.NetworkPlayer>();
                        }
                    }

                    for (int i = 0; i < _maxCount; i++)
                        SpawnNPC();

                    if (_civilians == null || _civilians.Length == 0)
                    {
                        _civilians = FindObjectsOfType<Civilian>();
                    }
                    _playerConnected[NetworkManager.Singleton.LocalClientId] = true;
                }

                PlayerConnectedServerRpc(NetworkManager.Singleton.LocalClientId);
                print($"Client Rpc to Server:: clientId: {NetworkManager.Singleton.LocalClientId}");
            }
        }

        private void SpawnNPC()
        {
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

        private void SetCivilians()
        {
            if (_civilians == null || _civilians.Length == 0)
            {
                _civilians = FindObjectsOfType<Civilian>();
            }

            if (_civilians == null || _civilians.Length < _playerCivilian.Count) { return; }

            var keys = _playerCivilian.Keys.ToArray();
            for (int i = 0; i < keys.Length;)
            {
                var key = keys[i];
                while (true)
                {
                    ulong id = _civilians[Random.Range(1, _civilians.Length + 1)].UniqueId;
                    if (_playerCivilian.Values.Contains(id) == false)
                    {
                        _playerCivilian[key] = id;
                        SetControlledCivilianClientRpc(key, id);
                        break;
                    }
                }
                i++;
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
        private void SetControlledCivilianClientRpc(ulong clientId, ulong civilianId)
        {
            if (_civilians == null || _civilians.Length == 0)
            {
                _civilians = FindObjectsOfType<Civilian>();
            }

            if(NetworkManager.Singleton.LocalClientId != clientId) { return; }

            if (_ownerNetPlayer == null)
            {
                _ownerNetPlayer = FindObjectsOfType<NetworkObject>()
                    .Where((netObj) => netObj.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    .First().GetComponent<Player.NetworkPlayer>();
            }

            if(_ownerNetPlayer == null)
            {
                Debug.Log("Owner Network Player is null!..");
                return;
            }

            if (_ownerNetPlayer.OwnerClientId == clientId)
            {
                var civilian = _civilians.Where((c) => c.UniqueId == civilianId).First();

                if (civilian == null) { return; }

                _ownerNetPlayer.SetControlledCivilian(civilian.transform);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerConnectedServerRpc(ulong clientId)
        {
            if (_playerConnected.ContainsKey(clientId))
            {
                _playerConnected[clientId] = true;
                PlayerConnectedClientRpc(clientId);
                print($"Server:: Client {clientId} connected");
            }

            if (_playerConnected.ContainsValue(false) == false)
            {
                SetCivilians();
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
            print($"Client:: Client {clientId} connected");
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
