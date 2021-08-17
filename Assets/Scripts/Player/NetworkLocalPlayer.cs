using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class NetworkLocalPlayer : NetworkBehaviour
    {
        [Header("Game settings")]
        [SerializeField] private GameObject _localBodyPrefab = null;
        [SerializeField] private GameObject _remoteBodyPrefab = null;
        [Range(1f, 10f)]
        [SerializeField] private float _smoothness = 3f;
        [SerializeField] private float _positionMinStep = 0.15f;
        [SerializeField] private float _rotationMinStep = 1.5f;
        private Transform _localBody = null;
        private Transform _remoteBody = null;

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

        public void EnableBody(Vector3 position, Quaternion rotation)
        {
            if (IsOwner)
            {
                _localBody = Instantiate(_localBodyPrefab).transform;

                _localBody.SetPositionAndRotation(position, rotation);

                _position.Value = _localBody.position;
                _rotation.Value = _localBody.rotation;
            }
            else
            {
                _remoteBody = Instantiate(_remoteBodyPrefab).transform;

                _remoteBody.SetPositionAndRotation(_position.Value, _rotation.Value);
            }

            IsBodyEnabled = true;
        }

        public void DisableBody()
        {
            if (IsOwner)
            {
                if (_localBody != null)
                    Destroy(_localBody.gameObject);
            }
            else
            {
                if (_remoteBody != null)
                    Destroy(_remoteBody.gameObject);
            }

            IsBodyEnabled = false;
        }

        private void UpdateBody()
        {
            if (IsBodyEnabled == false) { return; }
            if (IsOwner)
            {
                if (Vector3.Distance(_localBody.position, _position.Value) > _positionMinStep)
                {
                    _position.Value = _localBody.position;
                }

                if (Quaternion.Angle(_localBody.rotation, _rotation.Value) > _rotationMinStep)
                {
                    _rotation.Value = _localBody.rotation;
                }
            }
            else
            {
                _remoteBody.SetPositionAndRotation(
                    Vector3.Lerp(_remoteBody.position, _position.Value, _smoothness * Time.deltaTime),
                    Quaternion.Lerp(_remoteBody.rotation, _rotation.Value, _smoothness * Time.deltaTime)
                    );
            }
        }
        #endregion Body
    }
}