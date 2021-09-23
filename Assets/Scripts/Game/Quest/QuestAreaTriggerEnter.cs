using UnityEngine;

namespace Assets.Scripts.Game.Quest
{
    [RequireComponent(typeof(Collider))]
    public class QuestAreaTriggerEnter : BaseQuestTrigger
    {
        private void OnTriggerEnter(Collider other)
        {
            if (IsTriggered) { return; }

            if (other.TryGetComponent(out QuestPlayerMarker marker))
            {
                Trigger();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (IsTriggered) { return; }

            if (other.TryGetComponent(out QuestPlayerMarker marker))
            {
                Trigger();
            }
        }
    }
}