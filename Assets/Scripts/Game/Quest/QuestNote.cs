using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Quest
{
    public class QuestNote : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _descriptionField = null;
        [SerializeField] private Image _stateIcon = null;
        [SerializeField] private Sprite _unknownSprite = null;
        [SerializeField] private Sprite _activeSprite = null;
        [SerializeField] private Sprite _completeSprite = null;

        private QuestController _targetQuest = null;
        public QuestController TargetQuest {
            get => _targetQuest;
            set
            {
                _targetQuest = value;

                if (_targetQuest == null)
                {
                    Description = "??? ???? ????? ???? ???";
                    _stateIcon.sprite = _unknownSprite;
                }
                else if (_targetQuest.IsComplete)
                {
                    Description = TargetQuest.CompleteDescription;
                    _stateIcon.sprite = _completeSprite;
                }
                else if (_targetQuest.IsActive)
                {
                    Description = TargetQuest.ActiveDescription;
                    _stateIcon.sprite = _activeSprite;
                }
                else
                {
                    Description = TargetQuest.UnknownDescription;
                    _stateIcon.sprite = _unknownSprite;
                }
            }
        }

        public string Description
        {
            get => _descriptionField.text;
            private set =>_descriptionField.text = value;
        }
    }
}