using Assets.Scripts.Loading;
using Assets.Scripts.Network;
using Assets.Scripts.Player;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.Transports.PhotonRealtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Lobby
{
    public class LobbyNetwork : NetworkBehaviour
    {
        [SerializeField] private PlayerProfile _profile = null;

        public NetworkManager Manager => NetworkManager.Singleton;
        public PhotonRealtimeTransport Transport => Manager.GetComponent<PhotonRealtimeTransport>();

        public static LobbyNetwork Singleton { get; private set; } = null;

        private string _room = string.Empty;
        private Dictionary<ulong, Network.NetworkActor> _clients = new Dictionary<ulong, Network.NetworkActor>();

        public event Action<Network.NetworkActor> OnLocalConnected;
        public event Action OnDisconnected;

        private ulong _id = 0;
        private ulong NextId => ++_id;

        public readonly NetworkVariableBool IsAllReady = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);

            _clients.Clear();
            SubscribeToNetworkManager();
        }

        private void Start()
        {
            if (Manager.IsClient)
            {
                _room = Transport.RoomName;

                if (Manager.IsHost)
                {
                    foreach (var n_client in Manager.ConnectedClientsList)
                    {
                        NetworkActor client = n_client.PlayerObject.GetComponent<NetworkActor>();
                        if (_clients.ContainsValue(client) == false)
                        {
                            _clients.Add(client.ID.Value, client);
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromNetworkManager();
        }

        private void SpawnCard(Network.NetworkActor client)
        {
            if (client.IsOwner == false) { return; }

            if (client.CardArchitector.IsLocalSpawned.Value == false)
            {
                PlayerPanel playerPanel = FindObjectOfType<PlayerPanel>();

                if (playerPanel == null)
                {
                    throw new NullReferenceException("Object with component of type \"PlayerPanel\" not found");
                }

                client.CardArchitector.SpawnLocal(playerPanel.Container);
                (client.CardArchitector.Local as LocalCard).Card.Profile = _profile;
            }
        }

        #region Host

        public void StartHost(string room)
        {
            _clients.Clear();
            _room = room;
            Transport.RoomName = _room;

            SubscribeToNetworkManager();
            Manager.StartHost();
        }

        private void StopHost()
        {
            if (IsHost == false) { return; }

            Manager.ConnectionApprovalCallback -= ApprovalCheck;
            Manager.StopHost();
            UnsubscribeFromNetworkManager();

            _room = string.Empty;
            _clients.Clear();
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkManager.ConnectionApprovedDelegate callback)
        {
            string password = System.Text.Encoding.ASCII.GetString(connectionData);
            string check = _room;

            bool approve = check == password;
            bool createPlayerObject = true;

            // The prefab hash. Use null to use the default player prefab
            // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkPrefabs field of your NetworkManager object in the scene
            //ulong? prefabHash = NetworkSpawnManager.GetPrefabHashFromGenerator("MyPrefabHashGenerator");
            ulong? prefabHash = null;

            //If approve is true, the connection gets added. If it's false. The client gets disconnected
            callback(createPlayerObject, prefabHash, approve, Vector3.zero, Quaternion.identity);
        }

        #endregion Host

        #region Client

        public void StartClient(string room)
        {
            _clients.Clear();
            _room = room;
            Transport.RoomName = _room;

            Manager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(_room);
            Manager.StartClient();
        }

        private void StopClient()
        {
            if (IsHost) { return; }

            Manager.StopClient();

            _room = string.Empty;
            _clients.Clear();
        }

        #endregion Client

        public void Leave()
        {
            if (Manager.IsHost)
            {
                StopHost();
            }
            else if (Manager.IsClient)
            {
                StopClient();
            }
        }

        public void StartGame()
        {
            if (IsHost == false) { return; }
            if (IsAllReady.Value == false) { return; }

            LoadingManager.Singleton.LoadGame();
        }

        #region NetworkManager Subscribing

        private bool _subscribedToNetworkManager = false;
        private void SubscribeToNetworkManager()
        {
            if (Manager == null) { return; }
            if (_subscribedToNetworkManager) { return; }

            Manager.ConnectionApprovalCallback += ApprovalCheck;

            Manager.OnServerStarted += OnServerStarted;
            Manager.OnClientConnectedCallback += OnClientConnected;
            Manager.OnClientDisconnectCallback += OnClientDisconnected;

            _subscribedToNetworkManager = true;
        }

        private void UnsubscribeFromNetworkManager()
        {
            if (Manager == null) { return; }
            if (_subscribedToNetworkManager == false) { return; }

            Manager.ConnectionApprovalCallback -= ApprovalCheck;

            Manager.OnServerStarted -= OnServerStarted;
            Manager.OnClientConnectedCallback -= OnClientConnected;
            Manager.OnClientDisconnectCallback -= OnClientDisconnected;

            _subscribedToNetworkManager = false;
        }

        #endregion NetworkManager Subscribing

        #region NetworkManager Actions

        private void OnServerStarted()
        {
            if (Manager == null) { return; }
            if (Manager.IsServer == false) { return; }

            foreach (var pair in Manager.ConnectedClients)
            {

                NetworkActor client = pair.Value.PlayerObject.GetComponent<NetworkActor>();
                if (_clients.ContainsValue(client)) { break; }

                OnClientConnected(pair.Key);
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            if (Manager == null) { return; }
            if (Manager.IsServer == false) { return; }

            NetworkActor client = Manager.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkActor>();

            if (_clients.ContainsValue(client)) { return; }

            ulong id = 0;
            do
            {
                id = NextId;
            } while (_clients.ContainsKey(id));

            client.ID.Value = id;
            _clients.Add(id, client);

            SpawnCard(client);

            if (client.IsOwner) OnLocalConnected?.Invoke(client);

            OnClientConnectedClientRpc(id);

            CheckAllReadyServerRpc();
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (Manager == null) { return; }
            if (Manager.IsServer == false) { return; }

            var removeKeys = _clients.Where((a) => a.Value == null).Select((b) => b.Key);
            foreach (var key in removeKeys)
            {
                _clients.Remove(key);
            }

            if (_clients.ContainsKey(clientId))
            {
                _clients.Remove(clientId);
            }

            OnDisconnected?.Invoke();

            OnClientDisconnectedClientRpc(clientId);

            CheckAllReadyServerRpc();
        }

        #endregion NetworkManager Actions

        #region Messaging

        [ClientRpc]
        private void OnClientConnectedClientRpc(ulong clientId)
        {
            if (Manager == null) { return; }
            if (Manager.IsServer) { return; }

            NetworkActor client = FindObjectsOfType<NetworkActor>().First((n) => n.ID.Value == clientId);

            if (client == null) { return; }
            if (_clients.ContainsKey(clientId)) { return; }

            _clients.Add(clientId, client);
            SpawnCard(client);

            if (client.IsOwner) OnLocalConnected?.Invoke(client);
        }

        [ClientRpc]
        private void OnClientDisconnectedClientRpc(ulong clientId)
        {
            if (Manager == null) { return; }
            if (Manager.IsServer) { return; }

            var removeKeys = _clients.Where((a) => a.Value == null).Select((b) => b.Key);
            foreach (var key in removeKeys)
            {
                _clients.Remove(key);
            }

            if (_clients.ContainsKey(clientId))
            {
                _clients.Remove(clientId);
            }

            OnDisconnected?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        public void CheckAllReadyServerRpc()
        {
            foreach (var client in _clients.Values)
            {
                if (client.CardArchitector.IsLocalSpawned.Value)
                {
                    bool isReady = (client.CardArchitector as CardArchitector).IsReady.Value;
                    if (isReady == false)
                    {
                        if (IsAllReady.Value) IsAllReady.Value = false;
                        return;
                    }
                }
                else
                {
                    if (IsAllReady.Value) IsAllReady.Value = false;
                    return;
                }
            }
            if (IsAllReady.Value == false) IsAllReady.Value = true;
        }

        #endregion Messaging
    }
}