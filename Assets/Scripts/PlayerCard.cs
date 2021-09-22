using Assets.Scripts.TestLogic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lobby
{
    public class PlayerCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nickname = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private GameObject _ownerReadyFlag = null;
        [SerializeField] private GameObject _ownerNotReadyFlag = null;
        [SerializeField] private GameObject _notOwnerReadyFlag = null;
        [SerializeField] private GameObject _notOwnerNotReadyFlag = null;

        private PlayerProfile _profile = null;
        public PlayerProfile Profile
        {
            get => _profile;
            set
            {
                _profile = value;

                if (_profile == null)
                {
                    Nickname = "Unknown";
                    IconIndex = 0;
                    IsReady = false;
                }
                else
                {
                    Nickname = _profile.Nickname;
                    IconIndex = _profile.IconIndex;
                    IsReady = false;
                }
            }
        }

        public string Nickname
        {
            get => _nickname.text;
            set
            {
                _nickname.text = value;
                NicknameChanged?.Invoke(_nickname.text);
            }
        }
        public event Action<string> NicknameChanged = null;

        public Sprite Icon
        {
            get => _icon.sprite;
            set
            {
                _icon.sprite = value;
                IconChanged?.Invoke(_icon.sprite);
            }
        }
        public event Action<Sprite> IconChanged = null;

        private int _iconIndex = 0;
        public int IconIndex
        {
            get => _iconIndex;
            set
            {
                if (_iconIndex != value)
                {
                    _iconIndex = value;
                    _icon.sprite = Resources.Load<Sprite>($"Player_{_iconIndex}");
                    IconIndexChanged?.Invoke(_iconIndex);
                }
            }
        }
        public event Action<int> IconIndexChanged = null;

        public bool IsReady
        {
            get => _ownerReadyFlag.activeSelf;
            set
            {
                switch (Style)
                {
                    case PlayerCardStyle.OWNER:
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
                    case PlayerCardStyle.NOTOWNER:
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
                IsReadyChanged?.Invoke(value);
            }
        }
        public event Action<bool> IsReadyChanged = null;

        private PlayerCardStyle _style = PlayerCardStyle.NOTOWNER;
        public PlayerCardStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                IsReady = IsReady;
                OnStyleChanged?.Invoke(_style);
            }
        }
        public event UnityAction<PlayerCardStyle> OnStyleChanged = null;
    }

    public enum PlayerCardStyle { OWNER, NOTOWNER }
}
