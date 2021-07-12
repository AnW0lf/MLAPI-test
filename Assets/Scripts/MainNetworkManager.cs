using UnityEngine;
using MLAPI;
using TMPro;
using MLAPI.Transports.PhotonRealtime;
using UnityEngine.UI;

public class MainNetworkManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nickNameField = null;
    [SerializeField] private TMP_InputField _roomNameField = null;
    [SerializeField] private GameObject _menu = null;

    public void CreateRoom()
    {
        _menu.SetActive(false);
        string nickname = _nickNameField.text;
        //NetworkManager.Singleton.GetComponent<PhotonRealtimeTransport>().Client.NickName = nickname;
        string roomname = _roomNameField.text;
        //NetworkManager.Singleton.GetComponent<PhotonRealtimeTransport>().RoomName = roomname;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(Vector3.up + Vector3.forward * 3f, Quaternion.identity, true, null);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
    {
        bool approve = System.Text.Encoding.ASCII.GetString(connectionData) == "";
        callback(true, null, approve, Vector3.up, Quaternion.identity);
    }

    public void JoinRoom()
    {
        _menu.SetActive(false);
        string nickname = _nickNameField.text;
        //NetworkManager.Singleton.GetComponent<PhotonRealtimeTransport>().Client.NickName = nickname;
        string roomname = _roomNameField.text;
        //NetworkManager.Singleton.GetComponent<PhotonRealtimeTransport>().RoomName = roomname;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("");
        NetworkManager.Singleton.StartClient();
    }
}
