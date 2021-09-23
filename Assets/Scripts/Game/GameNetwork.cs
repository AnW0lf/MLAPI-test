using MLAPI;
using UnityEngine;
using System.Collections.Generic;
using Network = Assets.Scripts.TestLogic.Network;

namespace Assets.Scripts.Game
{
    public class GameNetwork : NetworkBehaviour
    {
        [SerializeField] private Placer _placer = null;

        private readonly Dictionary<ulong, Network> _clients = new Dictionary<ulong, Network>();
        private ulong _id = 0;
        private ulong NextId
        {
            get
            {
                do
                {
                    _id++;
                } while (_clients.ContainsKey(_id));
                return _id;
            }
        }

        public NetworkManager Manager => NetworkManager.Singleton;

        public static GameNetwork Singleton { get; private set; } = null;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(Singleton);
        }

        public override void NetworkStart()
        {
            _clients.Clear();
            if (Manager.IsClient)
            {
                foreach (var n_client in Manager.ConnectedClientsList)
                {
                    Network client = n_client.PlayerObject.GetComponent<Network>();
                    if (_clients.ContainsValue(client) == false)
                    {
                        _clients.Add(client.ID.Value, client);
                        if (client.IsOwner)
                        {
                            client.BodyArchitector.SpawnLocal();
                            _placer.Place(client.BodyArchitector.Local.transform);
                        }
                    }
                }
            }
        }


    }
}
