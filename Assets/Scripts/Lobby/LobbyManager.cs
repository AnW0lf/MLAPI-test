using MLAPI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.TestLogic;
using Network = Assets.Scripts.TestLogic.Network;

namespace Assets.Scripts.Lobby
{
    public class LobbyManager : MonoBehaviour
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

        public void CreateRoom()
        {
            SubscribeToServer();

            RoomName = $"{Random.Range(1000, 10000)}";
            Server.Singleton.StartHost(RoomName);

            UpdateLobbyButtonsState();
        }

        public void JoinRoom()
        {
            SubscribeToServer();

            RoomName = if_roomName.text;
            Server.Singleton.StartClient(RoomName);

            UpdateLobbyButtonsState();
        }

        public void Leave()
        {
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    RoomName = string.Empty;
                    Server.Singleton.StopHost();
                }
                else
                {
                    RoomName = string.Empty;
                    Server.Singleton.StopClient();
                }
            }

            Card = null;
            UnsubscribeFromServer();
            _animator.SetTrigger("Menu");
        }

        public void ChangeReady()
        {
            if (Card == null) { return; }
            Card.IsReady = !Card.IsReady;

            UpdateLobbyButtonsState();
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
                        btn_Start.interactable = Server.Singleton.IsAllReady.Value;
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
            if (Server.Singleton == null) { return; }
            if (_subscribedToServer) { return; }

            Server.Singleton.OnLocalConnected += OnConnected;
            Server.Singleton.OnDisconnected += OnDisconnected;
            Server.Singleton.IsAllReady.OnValueChanged += OnIsAllReady;

            _subscribedToServer = true;
        }

        private void UnsubscribeFromServer()
        {
            if (Server.Singleton == null) { return; }
            if (_subscribedToServer == false) { return; }

            Server.Singleton.OnLocalConnected -= OnConnected;
            Server.Singleton.OnDisconnected -= OnDisconnected;
            Server.Singleton.IsAllReady.OnValueChanged -= OnIsAllReady;

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
