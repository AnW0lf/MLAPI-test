using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lobby
{
    public class PlayerListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nickname = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private GameObject _ownerReadyFlag = null;
        [SerializeField] private GameObject _ownerNotReadyFlag = null;
        [SerializeField] private GameObject _notOwnerReadyFlag = null;
        [SerializeField] private GameObject _notOwnerNotReadyFlag = null;

        public string Nickname
        {
            get => _nickname.text;
            set
            {
                _nickname.text = value;
                OnNicknameChanged?.Invoke(_nickname.text);
            }
        }
        public event UnityAction<string> OnNicknameChanged = null;

        public Sprite Icon
        {
            get => _icon.sprite;
            set
            {
                _icon.sprite = value;
                OnIconChanged?.Invoke(_icon.sprite);
            }
        }
        public event UnityAction<Sprite> OnIconChanged = null;

        public bool IsReady
        {
            get => _ownerReadyFlag.activeSelf;
            set
            {
                switch (Style)
                {
                    case PlayerListItemStyle.OWNER:
                        {
                            if (value)
                            {
                                _ownerReadyFlag.SetActive(true);
                                _ownerNotReadyFlag.SetActive(false);
                                _notOwnerReadyFlag.SetActive(false);
                                _notOwnerNotReadyFlag.SetActive(false);
                            }
                            else
                            {
                                _ownerReadyFlag.SetActive(false);
                                _ownerNotReadyFlag.SetActive(true);
                                _notOwnerReadyFlag.SetActive(false);
                                _notOwnerNotReadyFlag.SetActive(false);
                            }
                        }
                        break;
                    case PlayerListItemStyle.NOTOWNER:
                        {
                            if (value)
                            {
                                _ownerReadyFlag.SetActive(false);
                                _ownerNotReadyFlag.SetActive(false);
                                _notOwnerReadyFlag.SetActive(true);
                                _notOwnerNotReadyFlag.SetActive(false);
                            }
                            else
                            {
                                _ownerReadyFlag.SetActive(false);
                                _ownerNotReadyFlag.SetActive(false);
                                _notOwnerReadyFlag.SetActive(false);
                                _notOwnerNotReadyFlag.SetActive(true);
                            }
                        }
                        break;
                }
                OnReady?.Invoke(value);
            }
        }
        public event UnityAction<bool> OnReady = null;

        private PlayerListItemStyle _style = PlayerListItemStyle.NOTOWNER;
        public PlayerListItemStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                IsReady = IsReady;
                OnStyleChanged?.Invoke(_style);
            }
        }
        public event UnityAction<PlayerListItemStyle> OnStyleChanged = null;
    }

    public enum PlayerListItemStyle { OWNER, NOTOWNER }
}
