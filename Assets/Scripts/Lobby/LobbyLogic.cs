using MLAPI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Network = Assets.Scripts.Network.NetworkActor;
using Assets.Scripts.Network;

namespace Assets.Scripts.Lobby
{
    public class LobbyLogic : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [Header("RoomName")]
        [SerializeField] private TMP_InputField if_roomName = null;
        [SerializeField] private TextMeshProUGUI txt_roomName = null;
        [Header("Buttons")]
        [SerializeField] private Button btn_Leave = null;
        [SerializeField] private Button btn_Ready = null;
        [SerializeField] private Button btn_NotReady = null;
        [SerializeField] private Button btn_Start = null;

        private LobbyNetwork _lobbyNetwork = null;

        private string _roomName = string.Empty;
        public string RoomName
        {
            get => _roomName;
            set
            {
                _roomName = value;
                if_roomName.text = _roomName;
                txt_roomName.text = _roomName;
            }
        }

        private PlayerCard _card = null;
        public PlayerCard Card
        {
            get => _card;
            set
            {
                _card = value;
                UpdateLobbyButtonsState();
            }
        }

        private void Awake()
        {
            _lobbyNetwork = LobbyNetwork.Singleton;
        }

        private void Start()
        {
            if (_lobbyNetwork.Manager.IsClient)
            {
                _animator.SetTrigger("Lobby");
                RoomName = _lobbyNetwork.Transport.RoomName;
            }
        }

        private void FixedUpdate()
        {
            UpdateLobbyButtonsState();
        }

        public void CreateRoom()
        {
            if (_lobbyNetwork == null) { return; }

            SubscribeToServer();

            RoomName = $"{Random.Range(1000, 10000)}";
            _lobbyNetwork.StartHost(RoomName);

            UpdateLobbyButtonsState();
        }

        public void JoinRoom()
        {
            if (_lobbyNetwork == null) { return; }

            SubscribeToServer();

            RoomName = if_roomName.text;
            _lobbyNetwork.StartClient(RoomName);

            UpdateLobbyButtonsState();
        }

        public void Leave()
        {
            if (_lobbyNetwork == null) { return; }

            RoomName = string.Empty;
            _lobbyNetwork.Leave();

            Card = null;
            UnsubscribeFromServer();
            _animator.SetTrigger("Menu");
        }

        public void ChangeReady()
        {
            if (Card == null) { return; }
            Card.IsReady = !Card.IsReady;
            _lobbyNetwork.CheckAllReadyServerRpc();
            UpdateLobbyButtonsState();
        }

        public void StartGame()
        {
            if (_lobbyNetwork == null) { return; }

            _lobbyNetwork.StartGame();
        }

        private void UpdateLobbyButtonsState()
        {
            if (_lobbyNetwork == null) { return; }

            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsClient)
                {
                    btn_Leave.interactable = true;

                    if (Card != null)
                    {
                        btn_Ready.gameObject.SetActive(Card.IsReady == false);
                        btn_NotReady.gameObject.SetActive(Card.IsReady);
                    }
                    else
                    {
                        btn_Ready.gameObject.SetActive(false);
                        btn_NotReady.gameObject.SetActive(false);
                    }

                    if (NetworkManager.Singleton.IsHost)
                    {
                        btn_Start.gameObject.SetActive(true);
                        _lobbyNetwork.CheckAllReadyServerRpc();
                        btn_Start.interactable = _lobbyNetwork.IsAllReady.Value;
                    }
                    else
                    {
                        btn_Start.gameObject.SetActive(false);
                        btn_Start.interactable = false;
                    }
                }
                else
                {
                    btn_Leave.interactable = false;
                    btn_Ready.gameObject.SetActive(false);
                    btn_NotReady.gameObject.SetActive(false);
                    btn_Start.gameObject.SetActive(false);
                    btn_Start.interactable = false;
                }
            }
            else
            {
                btn_Leave.interactable = false;
                btn_Ready.gameObject.SetActive(false);
                btn_NotReady.gameObject.SetActive(false);
                btn_Start.gameObject.SetActive(false);
                btn_Start.interactable = false;
            }
        }

        private bool _subscribedToServer = false;
        private void SubscribeToServer()
        {
            if (_lobbyNetwork == null) { return; }
            if (_subscribedToServer) { return; }

            _lobbyNetwork.OnLocalConnected += OnConnected;
            _lobbyNetwork.OnDisconnected += OnDisconnected;
            _lobbyNetwork.IsAllReady.OnValueChanged += OnIsAllReady;

            _subscribedToServer = true;
        }

        private void UnsubscribeFromServer()
        {
            if (_lobbyNetwork == null) { return; }
            if (_subscribedToServer == false) { return; }

            _lobbyNetwork.OnLocalConnected -= OnConnected;
            _lobbyNetwork.OnDisconnected -= OnDisconnected;
            _lobbyNetwork.IsAllReady.OnValueChanged -= OnIsAllReady;

            _subscribedToServer = false;
        }

        private void OnConnected(NetworkActor client)
        {
            _client = client;
            _client.CardArchitector.IsLocalSpawned.OnValueChanged += OnCardSpawned;
            UpdateLobbyButtonsState();

            if (_client.CardArchitector.Local != null)
            {
                OnCardSpawned(false, _client.CardArchitector.IsLocalPlayer);
            }
        }

        private NetworkActor _client = null;

        private void OnCardSpawned(bool previousValue, bool newValue)
        {
            if (newValue == false)
            {
                Card = null;
                return;
            }

            _client.CardArchitector.IsLocalSpawned.OnValueChanged -= OnCardSpawned;
            Card = _client.CardArchitector.Local.GetComponent<PlayerCard>();
            _animator.SetTrigger("Lobby");
        }

        private void OnDisconnected()
        {
            UpdateLobbyButtonsState();

            if (Card == null)
            {
                UnsubscribeFromServer();
                _animator.SetTrigger("Menu");
            }
        }

        private void OnIsAllReady(bool previousValue, bool newValue)
        {
            UpdateLobbyButtonsState();
        }
    }
}
