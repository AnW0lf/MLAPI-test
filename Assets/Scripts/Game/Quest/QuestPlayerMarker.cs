using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Quest
{
    public class QuestPlayerMarker : MonoBehaviour
    {
        private ulong _playerId = 0;

        public ulong PlayerId => _playerId;

        public void SetPlayerId(ulong id)
        {
            _playerId = id;
        }
    }
}