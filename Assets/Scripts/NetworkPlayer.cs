using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private GameObject _body = null;
        [SerializeField] private GameObject _camera = null;
        [SerializeField] private Rig _rightHandRig = null;
        [SerializeField] private Weapon _weapon = null;

        private PlayerListItem _lobbyItem = null;
        private Collider _collider = null;
        private Rigidbody _rigidbody = null;

        private static NetworkVariableInt _iconOffset = new NetworkVariableInt(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, -1);

        private NetworkVariableBool _isWeaponVisible = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        public bool IsWeaponVisible
        {
            get => _isWeaponVisible.Value;
            set
            {
                if (IsOwner)
                {
                    _isWeaponVisible.Value = value;
                }
                else
                {
                    Debug.LogWarning($"{nameof(IsWeaponVisible)} can't be changed, because you are not owner!..");
                }
            }
        }

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
        #endregion PersonalData

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

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void NetworkStart()
        {
            Subscribe();

            if (IsServer && _iconOffset.Value < 0)
            {
                _iconOffset.Value = Random.Range(0, 10);
            }

            _lobbyItem = LobbyController.Singleton.AddPlayerListItem(OwnerClientId);

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

            if (LobbyController.Singleton != null)
            {
                LobbyController.Singleton.CheckAllReady();
                LobbyController.Singleton.ToLobby();
            }

        }

        private void OnDestroy()
        {
            Unsubscribe();
            if (LobbyController.Singleton != null)
            {
                LobbyController.Singleton.StartCoroutine(LobbyController.Singleton.DelayedCheckAllReady());
            }
        }

        private void Subscribe()
        {
            _iconPath.OnValueChanged += SetIcon;
            _nickname.OnValueChanged += SetNickname;
            _isReady.OnValueChanged += SetReady;

            _isWeaponVisible.OnValueChanged += SetWeaponVisibility;
        }

        private void Unsubscribe()
        {
            _iconPath.OnValueChanged -= SetIcon;
            _nickname.OnValueChanged -= SetNickname;
            _isReady.OnValueChanged -= SetReady;

            _isWeaponVisible.OnValueChanged -= SetWeaponVisibility;
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
            if (LobbyController.Singleton != null)
            {
                LobbyController.Singleton.CheckAllReady();
            }
        }

        private void SetWeaponVisibility(bool previousValue, bool newValue)
        {
            if(IsBodyActive == false) { return; }

            _rightHandRig.weight = newValue ? 1f : 0f;
            _weapon.gameObject.SetActive(newValue);
        }

        public void Shoot()
        {
            if(IsWeaponVisible == false) { return; }

            _weapon.Shoot();
        }

        public bool IsBodyActive
        {
            get => _body.activeSelf;
            set
            {
                if (IsBodyActive != value)
                {
                    if (IsOwner)
                    {
                        _playerController.Active = value;
                        _camera.SetActive(true);
                    }
                    _body.SetActive(value);
                    _collider.enabled = value;
                    _rigidbody.isKinematic = !value;
                    SetActiveBodyServerRpc(OwnerClientId, value);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetActiveBodyServerRpc(ulong clientId, bool active)
        {
            SetActiveBodyClientRpc(clientId, active);
        }

        [ClientRpc]
        private void SetActiveBodyClientRpc(ulong clientId, bool active)
        {
            if (clientId != OwnerClientId) { return; }

            IsBodyActive = active;
        }
    }
}