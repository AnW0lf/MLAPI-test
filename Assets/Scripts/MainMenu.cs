using UnityEngine;
using MLAPI;
using TMPro;
using System;
using System.Text;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _passwordInputField = null;
    [SerializeField] private GameObject _passwordEntryUI = null;
    [SerializeField] private GameObject _leaveButton = null;

    private string _password = "";

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    public void Host()
    {
        _password = _passwordInputField.text;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(Vector3.zero, Quaternion.identity);
    }

    public void Client()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(_passwordInputField.text);
        NetworkManager.Singleton.StartClient();
    }

    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }

        _passwordEntryUI.SetActive(true);
        _leaveButton.SetActive(false);
    }

    private void ApprovalCheck(byte[] connectionData, ulong cliendId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string password = Encoding.ASCII.GetString(connectionData);
        bool approveConnection = password == _password;
        callback(true, null, approveConnection, Vector3.zero, Quaternion.identity);
    }

    private void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            _passwordEntryUI.SetActive(false);
            _leaveButton.SetActive(true);
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            _passwordEntryUI.SetActive(true);
            _leaveButton.SetActive(false);
        }
    }
}
