using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Game.Quest
{
    public class QuestController : MonoBehaviour
    {
        [Header("Quest relations")]

        [Tooltip("Приоритет отображения." +
            "\nЗадания с меньшим приоритетом отображаются выше")]
        [SerializeField] private int _priorityIndex = 0;
        public int PriorityIndex => _priorityIndex;

        [Tooltip("Ссылка на следующие задания." +
            "\nЕсли это последнее задание," +
            "\nто массив остаётся пустой")]
        [SerializeField] private QuestController[] _neighbourQuests = null;

        [Tooltip("Ссылка на следующие задания." +
            "\nЕсли это последнее задание," +
            "\nто массив остаётся пустой")]
        [SerializeField] private QuestController[] _nextQuests = null;

        [Space(25)]

        [Header("Description")]

        [Tooltip("Текстовое описание задания пока оно не активно и не выполнено")]
        [TextArea(1, 10)]
        [SerializeField] private string _unknownDescription = "??? ???? ????? ???? ???";
        [Tooltip("Текстовое описание задания при активации")]
        [TextArea(1, 10)]
        [SerializeField] private string _activeDescription = string.Empty;
        [Tooltip("Текстовое описание выполненого задания")]
        [TextArea(1, 10)]
        [SerializeField] private string _completeDescription = string.Empty;
        public string UnknownDescription => _unknownDescription;
        public string ActiveDescription => _activeDescription;
        public string CompleteDescription => _completeDescription;

        [Tooltip("Контроллер условия срабатывания задания")]
        [SerializeField] private BaseQuestTrigger _trigger = null;

        public bool IsActive => enabled;
        public bool IsComplete { get; private set; }

        public event UnityAction<QuestController> OnActive;
        public event UnityAction<QuestController> OnDeactive;

        private void Start()
        {
            if(_trigger == null)
            {
                Debug.LogError($"Object {name} has field {nameof(_trigger)} is null on \"Start\" method!..");
            }

            _trigger.enabled = IsActive;
            if (_trigger.enabled) Subscribe();

            OnActive?.Invoke(this);
        }

        private void OnEnable()
        {
            if (_trigger == null)
            {
                Debug.LogError($"Object {name} has field {nameof(_trigger)} is null on \"OnEnable\" method!..");
            }

            _trigger.enabled = true;
            Subscribe();

            OnActive?.Invoke(this);
        }

        private void OnDisable()
        {
            if (_trigger == null)
            {
                Debug.LogError($"Object {name} has field {nameof(_trigger)} is null on \"OnDisable\" method!..");
            }

            Unsubscribe();
            _trigger.enabled = false;

            OnDeactive?.Invoke(this);
        }

        private void OnDestroy()
        {
            if (_trigger == null)
            {
                Debug.LogError($"Object {name} has field {nameof(_trigger)} is null on \"OnDestroy\" method!..");
            }

            Unsubscribe();
            _trigger.enabled = false;

            OnDeactive?.Invoke(this);
        }

        private bool _subscribed = false;
        private void Subscribe()
        {
            if(_subscribed) { return; }
            _trigger.OnTriggered += Next;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (_subscribed == false) { return; }
            _trigger.OnTriggered -= Next;
            _subscribed = false;
        }

        public void Next(BaseQuestTrigger trigger)
        {
            if (trigger != _trigger) { return; }

            IsComplete = true;

            if (_neighbourQuests != null
                && _neighbourQuests.Length > 0
                && _neighbourQuests.Any((neighbour) => neighbour.IsComplete == false))
            {
                return;
            }

            foreach (var next in _nextQuests)
            {
                next.enabled = true;
            }

            foreach (var neighbour in _neighbourQuests)
            {
                neighbour.enabled = false;
            }

            this.enabled = false;
        }
    }
}