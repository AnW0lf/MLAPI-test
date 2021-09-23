using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.Quest
{
    public class QuestsDecorator : MonoBehaviour
    {
        [SerializeField] private GameObject _questNotePrefab = null;
        [SerializeField] private Transform _questNotesContainer = null;

        private List<QuestController> _quests = new List<QuestController>();
        private List<QuestController> _activeQuests = new List<QuestController>();
        private List<QuestNote> _questNotes = new List<QuestNote>();

        private void Start()
        {
            var quests = FindObjectsOfType<QuestController>();
            SetQuests(quests);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void SetQuests(QuestController[] quests)
        {
            Unsubscribe();
            _quests.Clear();

            _quests.AddRange(quests);
            Subscribe();

            foreach (var quest in _quests.Where((q) => q.IsActive || q.IsComplete))
            {
                AddToVisible(quest);
            }

            UpdateNotes();
        }

        private bool _subscribed = false;
        private void Subscribe()
        {
            if (_subscribed) { return; }

            if (_quests == null || _quests.Count == 0) { return; }

            foreach (var quest in _quests)
            {
                if (quest != null)
                {
                    quest.OnActive += AddToVisible;
                    quest.OnDeactive += RemoveFromVisible;
                }
            }
        }

        private void Unsubscribe()
        {
            if (_subscribed == false) { return; }

            if (_quests == null || _quests.Count == 0) { return; }

            foreach (var quest in _quests)
            {
                if (quest != null)
                {
                    quest.OnActive -= AddToVisible;
                    quest.OnDeactive -= RemoveFromVisible;
                }
            }
        }

        private void AddToVisible(QuestController quest)
        {
            if (quest == null) { return; }
            if (_activeQuests == null) { return; }

            if (_activeQuests.Contains(quest) == false)
            {
                _activeQuests.Add(quest);
            }

            _activeQuests.Sort((first, second) => first.PriorityIndex.CompareTo(second.PriorityIndex));

            UpdateNotes();
        }

        private void RemoveFromVisible(QuestController quest)
        {
            if (quest == null) { return; }
            if (_activeQuests == null || _activeQuests.Count == 0) { return; }

            if (quest.IsComplete == false)
            {
                if (_activeQuests.Contains(quest))
                {
                    _activeQuests.Remove(quest);
                }
            }

            UpdateNotes();
        }

        private void UpdateNotes()
        {
            if (_questNotes == null)
            {
                Debug.LogWarning($"UpdateNotes:: {nameof(_questNotes)} is null");
                _questNotes = new List<QuestNote>();
            }

            if (_activeQuests == null)
            {
                Debug.LogWarning($"UpdateNotes:: {nameof(_activeQuests)} is null");
                _activeQuests = new List<QuestController>();

                for (int i = _questNotes.Count - 1; i >= 0; i--)
                {
                    var note = _questNotes[i];
                    _questNotes.RemoveAt(i);
                    Destroy(note.gameObject);
                }

                return;
            }

            while (_activeQuests.Count != _questNotes.Count)
            {
                if (_activeQuests.Count > _questNotes.Count)
                {
                    var note = Instantiate(_questNotePrefab, _questNotesContainer).GetComponent<QuestNote>();
                    _questNotes.Add(note);
                }
                else
                {
                    var note = _questNotes.Last();
                    _questNotes.Remove(note);
                    if (note != null)
                    {
                        Destroy(note.gameObject);
                    }
                }
            }

            for (int i = 0; i < _activeQuests.Count; i++)
            {
                var note = _questNotes[i];
                var activeQuest = _activeQuests[i];

                note.TargetQuest = activeQuest;
            }
        }
    }
}