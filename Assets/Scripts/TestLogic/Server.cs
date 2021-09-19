using MLAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.TestLogic
{
    public class Server : MonoBehaviour
    {
        private Dictionary<ulong, Network> _networks = new Dictionary<ulong, Network>();
        private ulong _id = 0;

        private void Awake()
        {
            SubscribeToNetworkManager();   
        }

        private void OnEnable()
        {
            SubscribeToNetworkManager();
        }

        private void OnDisable()
        {
            UnsubscribeFromNetworkManager();
        }

        private void OnDestroy()
        {
            UnsubscribeFromNetworkManager();
        }

        private bool _subscribedToNetworkManager = false;
        private void SubscribeToNetworkManager()
        {
            if (NetworkManager.Singleton == null) { return; }
            if (_subscribedToNetworkManager) { return; }

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            _subscribedToNetworkManager = true;
        }

        private void UnsubscribeFromNetworkManager()
        {
            if (NetworkManager.Singleton == null) { return; }
            if (_subscribedToNetworkManager == false) { return; }
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            _subscribedToNetworkManager = false;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton == null) { return; }
            if (NetworkManager.Singleton.IsServer == false) { return; }

            Network client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Network>();
            print(client);

            if(_networks.ContainsValue(client) == false)
            {
                ulong id = NextId;
                _networks.Add(id, client);
            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (NetworkManager.Singleton == null) { return; }
            if (NetworkManager.Singleton.IsServer == false) { return; }

            Network client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Network>();
            print(client);

            var removeKeys = _networks.Where((a) => a.Value == null).Select((b) => b.Key);
            foreach (var key in removeKeys)
                _networks.Remove(key);
        }

        private ulong NextId => ++_id;
    }
}