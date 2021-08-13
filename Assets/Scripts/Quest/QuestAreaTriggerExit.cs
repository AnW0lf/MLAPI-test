using UnityEngine;

namespace Assets.Scripts.Quest
{
    [RequireComponent(typeof(Collider))]
    public class QuestAreaTriggerExit : BaseQuestTrigger
    {
        private void OnTriggerExit(Collider other)
        {
            if (IsTriggered) { return; }

            if (other.TryGetComponent(out QuestPlayerMarker marker))
            {
                Trigger();
            }
        }
    }
}