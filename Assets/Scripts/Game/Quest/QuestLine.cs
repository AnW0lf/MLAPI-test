using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Quest
{
    public class QuestLine : MonoBehaviour
    {
        [SerializeField] private QuestController[] _activeOnBegin = null;
            
        public void Begin()
        {
            foreach (var quest in _activeOnBegin)
            {
                quest.enabled = true;
            }
        }
    }
}