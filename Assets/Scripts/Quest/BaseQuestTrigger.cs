using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Quest
{
    public class BaseQuestTrigger : MonoBehaviour
    {
        public event UnityAction<BaseQuestTrigger> OnTriggered;
        public bool IsTriggered { get; private set; } = false;

        private void Start()
        {
            IsTriggered = false;
        }

        private void OnEnable()
        {
            IsTriggered = false;
        }

        protected virtual void Trigger()
        {
            if(IsTriggered) { return; }
            OnTriggered?.Invoke(this);
            IsTriggered = true;
        }
    }
}