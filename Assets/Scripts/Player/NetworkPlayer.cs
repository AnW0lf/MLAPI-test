using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private Transform _ownerBody = null;
        [SerializeField] private Transform _otherBody = null;

        private PlayerListItem _lobbyItem = null;

        private static NetworkVariableInt _iconOffset = new NetworkVariableInt(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, -1);

        #region PersonalData
        private NetworkVariableString _nickname = new NetworkVariableString(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, "Player_9");

        private NetworkVariableString _iconPath = new NetworkVariableString(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, "Player_9");

        private NetworkVariableBool _isReady = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        public string Nickname
        {
            get => _nickname.Value;
            set
            {
                if (IsOwner)
                {
                    _nickname.Value = value;
                }
                else
                {
                    Debug.LogWarning($"{nameof(Nickname)} can't be changed, because you are not owner!..");
                }
            }
        }

        public string IconPath
        {
            get => _iconPath.Value;
            set
            {
                if (IsOwner)
                {
                    _iconPath.Value = value;
                }
                else
                {
                    Debug.LogWarning($"{nameof(IconPath)} can't be changed, because you are not owner!..");
                }
            }
        }

        public bool IsReady
        {
            get => _isReady.Value;
            set
            {
                if (IsOwner)
                {
                    _isReady.Value = value;
                }
                else
                {
                    Debug.LogWarning($"{nameof(IsReady)} can't be changed, because you are not owner!..");
                }
            }
        }

        private void SetNickname(string previousValue, string newValue)
        {
            if (_lobbyItem != null)
            {
                _lobbyItem.Nickname = Nickname;
            }
        }

        private void SetIcon(string previousValue, string newValue)
        {
            if (_lobbyItem != null)
            {
                _lobbyItem.Icon = Resources.Load<Sprite>(IconPath);
            }
        }

        private void SetReady(bool previousValue, bool newValue)
        {
            if (_lobbyItem != null)
            {
                _lobbyItem.IsReady = IsReady;
            }
            if (LobbyManager.Singleton != null)
            {
                LobbyManager.Singleton.CheckAllReady();
            }
        }
        #endregion PersonalData

        public override void NetworkStart()
        {
            Subscribe();

            if (IsServer && _iconOffset.Value < 0)
            {
                _iconOffset.Value = Random.Range(0, 10);
            }

            _lobbyItem = LobbyManager.Singleton.AddPlayerListItem(OwnerClientId);

            if (IsOwner)
            {
                Nickname = $"Player_{OwnerClientId}";
                IconPath = $"Player_{((int)OwnerClientId + _iconOffset.Value) % 10}";
                IsReady = false;
                _lobbyItem.Style = PlayerListItemStyle.OWNER;
            }
            else
            {
                SetNickname("", Nickname);
                SetIcon("", IconPath);
                SetReady(false, IsReady);
                _lobbyItem.Style = PlayerListItemStyle.NOTOWNER;
            }

            if (LobbyManager.Singleton != null)
            {
                LobbyManager.Singleton.CheckAllReady();
                LobbyManager.Singleton.ToLobby();
            }

        }

        private void Update()
        {
            UpdateBody();
        }

        private void OnDestroy()
        {
            Unsubscribe();
            if (LobbyManager.Singleton != null)
            {
                LobbyManager.Singleton.StartCoroutine(LobbyManager.Singleton.DelayedCheckAllReady());
            }
        }

        private void Subscribe()
        {
            _iconPath.OnValueChanged += SetIcon;
            _nickname.OnValueChanged += SetNickname;
            _isReady.OnValueChanged += SetReady;
        }

        private void Unsubscribe()
        {
            _iconPath.OnValueChanged -= SetIcon;
            _nickname.OnValueChanged -= SetNickname;
            _isReady.OnValueChanged -= SetReady;
        }

        #region Body
        public bool IsBodyEnabled { get; private set; } = false;

        private NetworkVariableVector3 _position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector3.zero);

        private NetworkVariableQuaternion _rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Quaternion.identity);

        public void EnableBody(Vector3 position, Quaternion rotation)
        {
            if (IsOwner)
            {
                _ownerBody.gameObject.SetActive(true);
                _otherBody.gameObject.SetActive(false);

                _ownerBody.position = position;
                _ownerBody.rotation = rotation;

                _position.Value = _ownerBody.position;
                _rotation.Value = _ownerBody.rotation;
            }
            else
            {
                _otherBody.gameObject.SetActive(true);
                _ownerBody.gameObject.SetActive(false);

                _otherBody.position = position;
                _otherBody.rotation = rotation;
            }
            IsBodyEnabled = true;
        }

        public void DisableBody()
        {
            if (IsOwner)
            {
                _ownerBody.gameObject.SetActive(false);
                _otherBody.gameObject.SetActive(false);
            }
            else
            {
                _otherBody.gameObject.SetActive(false);
                _ownerBody.gameObject.SetActive(false);
            }
            IsBodyEnabled = false;
        }

        private void UpdateBody()
        {
            if(IsBodyEnabled == false) { return; }
            if (IsOwner)
            {
                if(Vector3.Distance(_ownerBody.position, _position.Value) > 0.15f)
                {
                    _position.Value = _ownerBody.position;
                }

                if (Quaternion.Angle(_ownerBody.rotation, _rotation.Value) > 1.5f)
                {
                    _rotation.Value = _ownerBody.rotation;
                }
            }
            else
            {
                _otherBody.position = Vector3.Lerp(_otherBody.position, _position.Value, 0.1f);
                _otherBody.rotation = Quaternion.Lerp(_otherBody.rotation, _rotation.Value, 0.1f);
            }
        }
        #endregion Body
    }
}