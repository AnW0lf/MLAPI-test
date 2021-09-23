using MLAPI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Network = Assets.Scripts.TestLogic.Network;

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

        private void Start()
        {
            if (LobbyNetwork.Singleton.Manager.IsClient)
            {
                _animator.SetTrigger("Lobby");
                RoomName = LobbyNetwork.Singleton.Transport.RoomName;
            }
        }

        public void CreateRoom()
        {
            SubscribeToServer();

            RoomName = $"{Random.Range(1000, 10000)}";
            LobbyNetwork.Singleton.StartHost(RoomName);

            UpdateLobbyButtonsState();
        }

        public void JoinRoom()
        {
            SubscribeToServer();

            RoomName = if_roomName.text;
            LobbyNetwork.Singleton.StartClient(RoomName);

            UpdateLobbyButtonsState();
        }

        public void Leave()
        {
            RoomName = string.Empty;
            LobbyNetwork.Singleton.Leave();

            Card = null;
            UnsubscribeFromServer();
            _animator.SetTrigger("Menu");
        }

        public void ChangeReady()
        {
            if (Card == null) { return; }
            Card.IsReady = !Card.IsReady;
            LobbyNetwork.Singleton.CheckAllReadyServerRpc();
            UpdateLobbyButtonsState();
        }

        public void StartGame()
        {
            if (LobbyNetwork.Singleton == null) { return; }

            LobbyNetwork.Singleton.StartGame();
        }

        private void UpdateLobbyButtonsState()
        {
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
                        btn_Start.interactable = LobbyNetwork.Singleton.IsAllReady.Value;
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
            if (LobbyNetwork.Singleton == null) { return; }
            if (_subscribedToServer) { return; }

            LobbyNetwork.Singleton.OnLocalConnected += OnConnected;
            LobbyNetwork.Singleton.OnDisconnected += OnDisconnected;
            LobbyNetwork.Singleton.IsAllReady.OnValueChanged += OnIsAllReady;

            _subscribedToServer = true;
        }

        private void UnsubscribeFromServer()
        {
            if (LobbyNetwork.Singleton == null) { return; }
            if (_subscribedToServer == false) { return; }

            LobbyNetwork.Singleton.OnLocalConnected -= OnConnected;
            LobbyNetwork.Singleton.OnDisconnected -= OnDisconnected;
            LobbyNetwork.Singleton.IsAllReady.OnValueChanged -= OnIsAllReady;

            _subscribedToServer = false;
        }

        private void OnConnected(Network client)
        {
            _client = client;
            _client.CardArchitector.IsLocalSpawned.OnValueChanged += OnCardSpawned;
            UpdateLobbyButtonsState();

            if (_client.CardArchitector.Local != null)
            {
                OnCardSpawned(false, _client.CardArchitector.IsLocalPlayer);
            }
        }

        private Network _client = null;

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
