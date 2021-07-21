using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Transports.PhotonRealtime;
using MLAPI.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lobby
{
    public class LobbyController : MonoBehaviour
    {
        public static LobbyController Singleton { get; private set; } = null;

        [SerializeField] private Animator _animator = null;
        [SerializeField] private PhotonRealtimeTransport _transport = null;
        [Header("Lobby Main Menu")]
        [SerializeField] private TMP_InputField _roomNameInputField = null;
        [Header("Lobby Player List")]
        [SerializeField] private TextMeshProUGUI _roomNameText = null;
        [SerializeField] private Transform _playerListContainer = null;
        [SerializeField] private GameObject _playerListItemPrefab = null;
        private Dictionary<ulong, PlayerListItem> _playerList = new Dictionary<ulong, PlayerListItem>();
        [Header("Lobby ToGameButtons")]
        [SerializeField] private Button _btnStartGame = null;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(this);
        }

        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) { return; }

            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }

        public void Host()
        {
            _transport.RoomName = $"{Random.Range(1, 10000)}";
            _roomNameText.text = $"Room name: {_transport.RoomName}";
            NetworkManager.Singleton.StartHost();
        }

        public void Client()
        {
            _transport.RoomName = _roomNameInputField.text;
            _roomNameText.text = $"Room name: {_transport.RoomName}";
            NetworkManager.Singleton.StartClient();
        }

        public void Leave()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StopHost();
                HandleClientDisconnect(NetworkManager.Singleton.LocalClientId);
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StopClient();
            }

            ToMenu();

            _roomNameText.text = "";
            for (int i = _playerList.Count - 1; i >= 0; i--)
                RemovePlayerListItem(_playerList.Keys.ToArray()[i]);
        }

        public void StartGame()
        {
            print($"Load scene Game");
            NetworkSceneManager.SwitchScene("Game");
        }

        private void ToMenu()
        {
            _animator.SetTrigger("Menu");
        }

        public void ToLobby()
        {
            _animator.SetTrigger("Lobby");
        }

        private void HandleServerStarted()
        {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }

        private void HandleClientConnected(ulong clientId)
        {
            _btnStartGame.gameObject.SetActive(NetworkManager.Singleton.IsHost);
            _btnStartGame.interactable = false;
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            RemovePlayerListItem(clientId);
        }

        public PlayerListItem AddPlayerListItem(ulong clientId)
        {
            var item = Instantiate(_playerListItemPrefab, _playerListContainer).GetComponent<PlayerListItem>();
            item.IsReady = false;
            _playerList.Add(clientId, item);
            return item;
        }

        public void RemovePlayerListItem(ulong clientId)
        {
            var item = _playerList[clientId];
            _playerList.Remove(clientId);
            Destroy(item.gameObject);
        }

        public void ChangeReady()
        {
            foreach (var networkPlayer in FindObjectsOfType<Player.NetworkPlayer>())
            {
                if (networkPlayer.IsOwner)
                {
                    networkPlayer.IsReady = !networkPlayer.IsReady;
                }
            }
        }

        public void CheckAllReady()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                foreach (var networkPlayer in FindObjectsOfType<Player.NetworkPlayer>())
                {
                    if (networkPlayer.IsReady == false)
                    {
                        _btnStartGame.interactable = false;
                        return;
                    }
                }

                _btnStartGame.interactable = true;
            }
        }

        public IEnumerator DelayedCheckAllReady()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            CheckAllReady();
        }
    }
}
