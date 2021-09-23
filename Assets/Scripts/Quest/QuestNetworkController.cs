using MLAPI;
using MLAPI.Messaging;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Quest
{
    public class QuestNetworkController : NetworkBehaviour
    {
        [SerializeField] private QuestLine[] _questLines = null;

        private ulong[] _clientIds = null;

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _clientIds = new ulong[_questLines.Length];
                for (int i = 0; i < _clientIds.Length; _clientIds[i] = ulong.MaxValue, i++) ;
            }

            //GameNetwork.Singleton.OnBodyEnabled += RequestQuestLine;
        }

        private void RequestQuestLine()
        {
            RequestQuestLineServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestQuestLineServerRpc(ulong clientId)
        {
            int questLineIndex = RandomEmptyQuestLineIndex;

            if (questLineIndex == int.MaxValue)
            {
                throw new ArgumentException(
                    $"RequestQuestLineIndexException:: _questLine massive" +
                    $"does not contain empty element to send it to client"
                    );
            }

            _clientIds[questLineIndex] = clientId;
            RequestQuestLineClientRpc(clientId, questLineIndex);

        }

        [ClientRpc]
        private void RequestQuestLineClientRpc(ulong clientId, int index)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId) { return; }
            _questLines[index].Begin();
        }

        private int RandomEmptyQuestLineIndex
        {
            get
            {
                if (NetworkManager.Singleton.IsServer == false) { return int.MaxValue; }

                List<int> emptyIndexes = new List<int>();
                for (int i = 0; i < _clientIds.Length; i++)
                {
                    if (_clientIds[i] == ulong.MaxValue)
                    {
                        emptyIndexes.Add(i);
                    }
                }
                if (emptyIndexes.Count == 0) return int.MaxValue;
                return emptyIndexes[Random.Range(0, emptyIndexes.Count)];
            }
        }
    }
}