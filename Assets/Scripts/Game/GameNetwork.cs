using MLAPI;
using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts.Network;

namespace Assets.Scripts.Game
{
    public class GameNetwork : NetworkBehaviour
    {
        [SerializeField] private Placer _placer = null;
        [SerializeField] private NetworkNPCSet[] _networkNpcSet = null;

        private readonly Dictionary<ulong, Network.NetworkActor> _clients = new Dictionary<ulong, Network.NetworkActor>();
        private readonly Dictionary<ulong, Network.NetworkActor> _npcs = new Dictionary<ulong, Network.NetworkActor>();
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
                    NetworkActor client = n_client.PlayerObject.GetComponent<NetworkActor>();
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

                if (Manager.IsHost)
                {
                    foreach (var nnpcSet in _networkNpcSet)
                    {
                        for (int i = 0; i < nnpcSet.Count; i++)
                        {
                            SpawnNpc(nnpcSet.NetworkNPCPrefab);
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var pair in _npcs)
            {
                if (pair.Value != null)
                    Destroy(pair.Value.gameObject);
            }
            _npcs.Clear();
        }

        private void SpawnNpc(NetworkActor prefab)
        {
            NetworkActor npc = Instantiate(prefab);
            npc.GetComponent<NetworkObject>().Spawn();
            npc.BodyArchitector.SpawnLocal();
            npc.ID.Value = NextId;
            _npcs.Add(npc.ID.Value, npc);
            _placer.Place(npc.BodyArchitector.Local.transform);
        }
    }

    [Serializable]
    public class NetworkNPCSet
    {
        [SerializeField] private int _count;
        [SerializeField] private NetworkActor _npcPrefab;

        public NetworkNPCSet(int count, NetworkActor npcPrefab)
        {
            _count = count;
            _npcPrefab = npcPrefab;
        }

        public int Count => _count;
        public NetworkActor NetworkNPCPrefab => _npcPrefab;
    }
}
