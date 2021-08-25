using Lobby;
using MLAPI;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Player
{
    public class NetworkLocalPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject _localPrefab = null;
        [SerializeField] private GameObject _remotePrefab = null;

        private LocalPlayer _local = null;
        private RemotePlayer _remote = null;

        private PlayerListItem _lobbyItem = null;

        #region PersonalData
        private static readonly NetworkVariableInt _iconOffset = new NetworkVariableInt(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, -1);

        private readonly NetworkVariableString _nickname = new NetworkVariableString(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, "Player_9");

        private readonly NetworkVariableString _iconPath = new NetworkVariableString(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, "Player_9");

        private readonly NetworkVariableBool _isReady = new NetworkVariableBool(new NetworkVariableSettings
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

        public Transform Body => IsOwner ? _local.transform : _remote.transform;

        private readonly NetworkVariableVector3 _position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector3.zero);

        private readonly NetworkVariableQuaternion _rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Quaternion.identity);

        private readonly NetworkVariableVector2 _velocity = new NetworkVariableVector2(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector2.zero);

        public event Action<Vector3> OnPositionChanged;
        public event Action<Quaternion> OnRotationChanged;
        public event Action<Vector2> OnVelocityChanged;

        public void EnableBody(Vector3 position, Quaternion rotation)
        {
            if (IsOwner)
            {
                _local = Instantiate(_localPrefab).GetComponent<LocalPlayer>(); ;
                _local.NetworkParent = this;

                _local.transform.SetPositionAndRotation(position, rotation);

                _position.Value = _local.transform.position;
                _rotation.Value = _local.transform.rotation;
            }
            else
            {
                _remote = Instantiate(_remotePrefab).GetComponent<RemotePlayer>(); ;

                _remote.transform.SetPositionAndRotation(_position.Value, _rotation.Value);
                _remote.ConnectToNetworkPlayer(this);
            }

            IsBodyEnabled = true;
        }

        public void DisableBody()
        {
            if (IsOwner)
            {
                if (_local != null)
                    Destroy(_local.gameObject);

                _position.Value = Vector3.zero;
                _rotation.Value = Quaternion.identity;
                _velocity.Value = Vector2.zero;
            }
            else
            {
                if (_remote != null)
                    Destroy(_remote.gameObject);
            }

            IsBodyEnabled = false;
        }

        private void UpdateBody()
        {
            if (IsBodyEnabled == false) { return; }
            if (IsOwner)
            {
                if (_local == null) { return; }

                _position.Value = _local.transform.position;
                _rotation.Value = _local.transform.rotation;
                _velocity.Value = _local.Velocity;
            }
            else
            {
                OnPositionChanged?.Invoke(_position.Value);
                OnRotationChanged?.Invoke(_rotation.Value);
                OnVelocityChanged?.Invoke(_velocity.Value);
            }
        }
        #endregion Body
    }
}