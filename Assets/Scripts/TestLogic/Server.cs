using Assets.Scripts.Lobby;
using MLAPI;
using MLAPI.Messaging;
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
        [SerializeField] private bool _isHost = false;

        public static Server Singleton { get; private set; } = null;

        private Dictionary<ulong, Network> _networks = new Dictionary<ulong, Network>();

        private ulong _id = 0;
        private ulong NextId => ++_id;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);

            SubscribeToNetworkManager();
        }

        private void Start()
        {
            SceneType = SceneType.LOBBY;

            if (_isHost) StartHost();
            else StartClient();
        }

        #region SceneType

        private SceneType _sceneType = SceneType.EMPTY;
        public SceneType SceneType
        {
            get => _sceneType;
            set
            {
                _sceneType = value;
                UpdateClientState();

                if (_manager == null) { return; }
                if (_manager.IsServer == false) { return; }

                OnSceneTypeChangedClientRpc(_sceneType);
            }
        }

        private void SetClientState(Network client, SceneType sceneType)
        {
            if (client == null) { return; }
            if (client.IsOwner == false) { return; }

            switch (_sceneType)
            {
                case SceneType.EMPTY:
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
                case SceneType.LOBBY:
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
                case SceneType.GAME:
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
                SetClientState(pair.Value, _sceneType);
            }
        }

        #endregion SceneType

        #region Host

        public void StartHost()
        {
            SubscribeToNetworkManager();
            _manager.StartHost();
        }

        public void StopHost()
        {
            _manager.StopHost();
            UnsubscribeFromNetworkManager();
        }

        #endregion Host

        #region Client

        public void StartClient()
        {
            _manager.StartClient();
        }

        public void StopClient()
        {
            _manager.StopClient();
        }

        #endregion Client

        #region NetworkManager Subscribing

        private bool _subscribedToNetworkManager = false;
        private void SubscribeToNetworkManager()
        {
            if (_manager == null) { return; }
            if (_subscribedToNetworkManager) { return; }

            _manager.OnServerStarted += OnServerStarted;
            _manager.OnClientConnectedCallback += OnClientConnected;
            _manager.OnClientDisconnectCallback += OnClientDisconnected;

            _subscribedToNetworkManager = true;
        }

        private void UnsubscribeFromNetworkManager()
        {
            if (_manager == null) { return; }
            if (_subscribedToNetworkManager == false) { return; }

            _manager.OnClientConnectedCallback += OnClientConnected;
            _manager.OnClientDisconnectCallback += OnClientDisconnected;

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

            SetClientState(client, _sceneType);

            OnClientConnectedClientRpc(id);
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

            OnClientDisconnectedClientRpc(clientId);
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
            SetClientState(client, _sceneType);
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
        }

        [ClientRpc]
        private void OnSceneTypeChangedClientRpc(SceneType sceneType)
        {
            if (_manager == null) { return; }
            if (_manager.IsServer) { return; }

            SceneType = sceneType;
        }

        #endregion Messaging
    }

    public enum SceneType { EMPTY, LOBBY, GAME }
}