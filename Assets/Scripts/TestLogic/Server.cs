using Assets.Scripts.Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.Transports.PhotonRealtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.TestLogic
{
    public class Server : NetworkBehaviour
    {
        [SerializeField] private PlayerProfile _profile = null;
        [SerializeField] private NetworkManager _manager = null;
        [SerializeField] private PhotonRealtimeTransport _transport = null;

        public static Server Singleton { get; private set; } = null;

        private string _room = string.Empty;
        private Dictionary<ulong, Network> _networks = new Dictionary<ulong, Network>();

        public event Action<Network> OnLocalConnected;
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
            DontDestroyOnLoad(gameObject);

            SubscribeToNetworkManager();
        }

        private void OnApplicationQuit()
        {
            TryStop();
        }

        private void OnDestroy()
        {
            TryStop();
        }

        #region SceneType

        private GameMode _gameMode = GameMode.EMPTY;
        public GameMode Mode
        {
            get => _gameMode;
            set
            {
                _gameMode = value;
                UpdateClientState();

                if (_manager == null) { return; }
                if (_manager.IsServer == false) { return; }

                OnSceneTypeChangedClientRpc(_gameMode);
            }
        }

        private void SetClientState(Network client, GameMode sceneType)
        {
            _gameMode = sceneType;

            if (client == null) { return; }
            if (client.IsOwner == false) { return; }

            switch (sceneType)
            {
                case GameMode.EMPTY:
                    {
                        if (client.CardArchitector.IsLocalSpawned.Value == false)
                        {
                            client.CardArchitector.DeleteLocal();
                        }

                        if (client.BodyArchitector.IsLocalSpawned.Value == false)
                        {
                            client.BodyArchitector.DeleteLocal();
                        }
                    }
                    break;
                case GameMode.LOBBY:
                    {
                        if (client.BodyArchitector.IsLocalSpawned.Value == false)
                        {
                            client.BodyArchitector.DeleteLocal();
                        }

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
                    break;
                case GameMode.GAME:
                    {
                        if (client.CardArchitector.IsLocalSpawned.Value == false)
                        {
                            client.CardArchitector.DeleteLocal();
                        }

                        if (client.BodyArchitector.IsLocalSpawned.Value == false)
                        {
                            Vector2 circle = Random.insideUnitCircle;
                            Vector3 position = new Vector3(circle.x, 0f, circle.y) * 5f;
                            Quaternion rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));

                            client.BodyArchitector.SpawnLocal();
                            Local local = client.BodyArchitector.Local;
                            local.transform.position = position;
                            local.transform.rotation = rotation;
                        }
                    }
                    break;
            }
        }

        private void UpdateClientState()
        {
            foreach (var pair in _networks)
            {
                SetClientState(pair.Value, _gameMode);
            }
        }

        #endregion SceneType

        #region Host

        public void StartHost(string room)
        {
            _networks.Clear();
            _room = room;
            _transport.RoomName = _room;

            SubscribeToNetworkManager();
            _manager.StartHost();

            Mode = GameMode.LOBBY;
        }

        public void StopHost()
        {
            if (IsHost == false) { return; }

            _manager.ConnectionApprovalCallback -= ApprovalCheck;
            _manager.StopHost();
            UnsubscribeFromNetworkManager();

            _room = string.Empty;
            Mode = GameMode.EMPTY;
            _networks.Clear();
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkManager.ConnectionApprovedDelegate callback)
        {
            string password = System.Text.Encoding.ASCII.GetString(connectionData);
            string check = _room + (Mode == GameMode.GAME ? "_GAME" : string.Empty);

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
            _networks.Clear();
            _room = room;
            _transport.RoomName = _room;

            _manager.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(_room);
            _manager.StartClient();
        }

        public void StopClient()
        {
            if (IsHost) { return; }

            _manager.StopClient();

            _room = string.Empty;
            Mode = GameMode.EMPTY;
            _networks.Clear();
        }

        #endregion Client

        private void TryStop()
        {
            if (_manager.IsHost)
            {
                StopHost();
            }
            else if (_manager.IsClient)
            {
                StopClient();
            }
        }

        #region NetworkManager Subscribing

        private bool _subscribedToNetworkManager = false;
        private void SubscribeToNetworkManager()
        {
            if (_manager == null) { return; }
            if (_subscribedToNetworkManager) { return; }

            _manager.ConnectionApprovalCallback += ApprovalCheck;

            _manager.OnServerStarted += OnServerStarted;
            _manager.OnClientConnectedCallback += OnClientConnected;
            _manager.OnClientDisconnectCallback += OnClientDisconnected;

            _subscribedToNetworkManager = true;
        }

        private void UnsubscribeFromNetworkManager()
        {
            if (_manager == null) { return; }
            if (_subscribedToNetworkManager == false) { return; }

            _manager.ConnectionApprovalCallback -= ApprovalCheck;

            _manager.OnServerStarted -= OnServerStarted;
            _manager.OnClientConnectedCallback -= OnClientConnected;
            _manager.OnClientDisconnectCallback -= OnClientDisconnected;

            _subscribedToNetworkManager = false;
        }

        #endregion NetworkManager Subscribing

        #region NetworkManager Actions

        private void OnServerStarted()
        {
            if (_manager == null) { return; }
            if (_manager.IsServer == false) { return; }

            foreach (var pair in _manager.ConnectedClients)
            {

                Network client = pair.Value.PlayerObject.GetComponent<Network>();
                if (_networks.ContainsValue(client)) { break; }

                OnClientConnected(pair.Key);
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            if (_manager == null) { return; }
            if (_manager.IsServer == false) { return; }

            Network client = _manager.ConnectedClients[clientId].PlayerObject.GetComponent<Network>();

            if (_networks.ContainsValue(client)) { return; }

            ulong id = NextId;
            client.ID.Value = id;
            _networks.Add(id, client);

            SetClientState(client, _gameMode);

            if (client.IsOwner) OnLocalConnected?.Invoke(client);

            OnClientConnectedClientRpc(id);

            CheckAllReadyServerRpc();
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (_manager == null) { return; }
            if (_manager.IsServer == false) { return; }

            var removeKeys = _networks.Where((a) => a.Value == null).Select((b) => b.Key);
            foreach (var key in removeKeys)
            {
                _networks.Remove(key);
            }

            if (_networks.ContainsKey(clientId))
            {
                _networks.Remove(clientId);
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
            if (_manager == null) { return; }
            if (_manager.IsServer) { return; }

            Network client = FindObjectsOfType<Network>().First((n) => n.ID.Value == clientId);

            if (client == null) { return; }
            if (_networks.ContainsKey(clientId)) { return; }

            _networks.Add(clientId, client);
            SetClientState(client, _gameMode);

            if (client.IsOwner) OnLocalConnected?.Invoke(client);
        }

        [ClientRpc]
        private void OnClientDisconnectedClientRpc(ulong clientId)
        {
            if (_manager == null) { return; }
            if (_manager.IsServer) { return; }

            var removeKeys = _networks.Where((a) => a.Value == null).Select((b) => b.Key);
            foreach (var key in removeKeys)
            {
                _networks.Remove(key);
            }

            if (_networks.ContainsKey(clientId))
            {
                _networks.Remove(clientId);
            }

            OnDisconnected?.Invoke();
        }

        [ClientRpc]
        private void OnSceneTypeChangedClientRpc(GameMode sceneType)
        {
            if (_manager == null) { return; }
            if (_manager.IsServer) { return; }

            Mode = sceneType;
        }

        [ServerRpc(RequireOwnership = false)]
        public void CheckAllReadyServerRpc()
        {
            if (Mode != GameMode.LOBBY) return;

            foreach(var client in _networks.Values)
            {
                if (client.CardArchitector.IsLocalSpawned.Value)
                {
                    bool isReady = (client.CardArchitector as CardArchitector).IsReady.Value;
                    if (isReady == false)
                    {
                        if(IsAllReady.Value) IsAllReady.Value = false;
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

    public enum GameMode { EMPTY, LOBBY, GAME }
}