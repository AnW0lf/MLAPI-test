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

        private LobbyNetwork LN => LobbyNetwork.Singleton;

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
            if (LN.Manager.IsClient)
            {
                _animator.SetTrigger("Lobby");
                RoomName = LN.Transport.RoomName;
            }
        }

        private void FixedUpdate()
        {
            UpdateLobbyButtonsState();
        }

        public void CreateRoom()
        {
            if (LN == null) { return; }

            SubscribeToServer();

            RoomName = $"{Random.Range(1000, 10000)}";
            LN.StartHost(RoomName);

            UpdateLobbyButtonsState();
        }

        public void JoinRoom()
        {
            if (LN == null) { return; }

            SubscribeToServer();

            RoomName = if_roomName.text;
            LN.StartClient(RoomName);

            UpdateLobbyButtonsState();
        }

        public void Leave()
        {
            if (LN == null) { return; }

            RoomName = string.Empty;
            LN.Leave();

            Card = null;
            UnsubscribeFromServer();
            _animator.SetTrigger("Menu");
        }

        public void ChangeReady()
        {
            if (Card == null) { return; }
            Card.IsReady = !Card.IsReady;
            LN.CheckAllReadyServerRpc();
            UpdateLobbyButtonsState();
        }

        public void StartGame()
        {
            if (LN == null) { return; }

            LN.StartGame();
        }

        private void UpdateLobbyButtonsState()
        {
            if (LN == null) { return; }

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
                        LN.CheckAllReadyServerRpc();
                        btn_Start.interactable = LN.IsAllReady.Value;
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
            if (LN == null) { return; }
            if (_subscribedToServer) { return; }

            LN.OnLocalConnected += OnConnected;
            LN.OnDisconnected += OnDisconnected;
            LN.IsAllReady.OnValueChanged += OnIsAllReady;

            _subscribedToServer = true;
        }

        private void UnsubscribeFromServer()
        {
            if (LN == null) { return; }
            if (_subscribedToServer == false) { return; }

            LN.OnLocalConnected -= OnConnected;
            LN.OnDisconnected -= OnDisconnected;
            LN.IsAllReady.OnValueChanged -= OnIsAllReady;

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
