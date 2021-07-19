using UnityEngine;
using MLAPI;
using TMPro;
using System.Text;
using System;
using MLAPI.Transports.PhotonRealtime;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _roomNameInputField = null;
    [SerializeField] private TextMeshProUGUI _roomNameText = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private PhotonRealtimeTransport _transport = null;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    public void Host()
    {
        _transport.RoomName = $"{UnityEngine.Random.Range(1, 10000)}";
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
        _roomNameText.text = "";
    }

    private void ToMenu()
    {
        _animator.SetTrigger("Menu");
    }

    private void ToLobby()
    {
        _animator.SetTrigger("Lobby");
    }

    private void HandleServerStarted()
    {
        HandleClientConnected(NetworkManager.Singleton.LocalClientId);
    }

    private void HandleClientConnected(ulong clientId)
    {
        ToLobby();
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        ToMenu();
    }
}
