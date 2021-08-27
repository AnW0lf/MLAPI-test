using Lobby;
using MLAPI;
using MLAPI.Messaging;
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

        private void OnDestroy()
        {
            Unsubscribe();

            if (IsOwner) DeleteLocal();
            else DeleteRemote();
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
        public Transform Body => IsOwner ? _local.transform : _remote.transform;

        public bool IsSpawned => IsSpawnedLocal || IsSpawnedRemote;
        public bool IsSpawnedLocal => _local != null;
        public bool IsSpawnedRemote => _remote != null;

        #region Variables
        #region Transform
        private readonly NetworkVariableVector3 _position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector3.zero);
        public event Action<Vector3> PositionChanged;

        private readonly NetworkVariableQuaternion _rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Quaternion.identity);
        public event Action<Quaternion> RotationChanged;
        #endregion Transform

        #region Animator
        private readonly NetworkVariableFloat _inputHorizontal = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);
        public event Action<float> InputHorizontalChanged;

        private readonly NetworkVariableFloat _inputVertical = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);
        public event Action<float> InputVerticalChanged;

        private readonly NetworkVariableFloat _inputMagnitude = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);
        public event Action<float> InputMagnitudeChanged;

        private readonly NetworkVariableBool _isGrounded = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsGroundedChanged;

        private readonly NetworkVariableBool _isStrafing = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsStrafingChanged;

        private readonly NetworkVariableBool _isSprinting = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsSprintingChanged;

        private readonly NetworkVariableBool _isCrouching = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsCrouchingChanged;

        private readonly NetworkVariableFloat _groundDistance = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);
        public event Action<float> GroundDistanceChanged;
        #endregion Animator

        #region Hand
        NetworkVariableBool _isHandVisible = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsHandVisibleChanged;
        #endregion Hand
        #endregion Variables

        #region Local
        public void SpawnLocal(Vector3 position, Quaternion rotation)
        {
            if (IsSpawnedLocal) { return; }
            if (IsOwner == false) { return; }

            _local = Instantiate(_localPrefab, position, rotation).GetComponent<LocalPlayer>(); ;
            _local.NetworkParent = this;
            _position.Value = position;
            _rotation.Value = rotation;
            SubscribeToLocalNpc();

            SpawnRemoteServerRpc();
        }

        private bool _subscribedToLocal = false;
        private void SubscribeToLocalNpc()
        {
            if (IsSpawnedLocal == false) { return; }
            if (_subscribedToLocal) { return; }

            _local.PositionChanged += OnLocalPlayerPositionChanged;
            _local.RotationChanged += OnLocalPlayerRotationChanged;

            _local.InputHorizontalChanged += OnLocalInputHorizontalChanged;
            _local.InputVerticalChanged += OnLocalInputVerticalChanged;
            _local.InputMagnitudeChanged += OnLocalInputMagnitudeChanged;
            _local.GroundDistanceChanged += OnLocalGroundDistanceChanged;

            _local.IsCrouchingChanged += OnLocalIsCrouchingChanged;
            _local.IsGroundedChanged += OnLocalIsGroundedChanged;
            _local.IsSprintingChanged += OnLocalIsSprintingChanged;
            _local.IsStrafingChanged += OnLocalIsStrafingChanged;

            _local.IsHandVisibleChanged += OnLocalIsHandVisibleChanged;

            _subscribedToLocal = true;
        }

        private void UnsubscribeFromLocalNpc()
        {
            if (IsSpawnedLocal == false) { return; }
            if (_subscribedToLocal == false) { return; }

            _local.PositionChanged -= OnLocalPlayerPositionChanged;
            _local.RotationChanged -= OnLocalPlayerRotationChanged;

            _local.InputHorizontalChanged -= OnLocalInputHorizontalChanged;
            _local.InputVerticalChanged -= OnLocalInputVerticalChanged;
            _local.InputMagnitudeChanged -= OnLocalInputMagnitudeChanged;
            _local.GroundDistanceChanged -= OnLocalGroundDistanceChanged;

            _local.IsCrouchingChanged -= OnLocalIsCrouchingChanged;
            _local.IsGroundedChanged -= OnLocalIsGroundedChanged;
            _local.IsSprintingChanged -= OnLocalIsSprintingChanged;
            _local.IsStrafingChanged -= OnLocalIsStrafingChanged;

            _local.IsHandVisibleChanged -= OnLocalIsHandVisibleChanged;

            _subscribedToLocal = false;
        }

        #region Local - Actions
        #region Transform - Actions
        private void OnLocalPlayerPositionChanged(Vector3 position)
        {
            _position.Value = position;
        }

        private void OnLocalPlayerRotationChanged(Quaternion rotation)
        {
            _rotation.Value = rotation;
        }
        #endregion Transform - Actions

        #region Animator - Actions
        private void OnLocalInputHorizontalChanged(float inputHorizontal)
        {
            _inputHorizontal.Value = inputHorizontal;
        }

        private void OnLocalInputVerticalChanged(float inputVertical)
        {
            _inputVertical.Value = inputVertical;
        }

        private void OnLocalInputMagnitudeChanged(float inputMagnitude)
        {
            _inputMagnitude.Value = inputMagnitude;
        }

        private void OnLocalGroundDistanceChanged(float groundDistance)
        {
            _groundDistance.Value = groundDistance;
        }

        private void OnLocalIsCrouchingChanged(bool isCrouching)
        {
            _isCrouching.Value = isCrouching;
        }

        private void OnLocalIsGroundedChanged(bool isGrounded)
        {
            _isGrounded.Value = isGrounded;
        }

        private void OnLocalIsSprintingChanged(bool isSprinting)
        {
            _isSprinting.Value = isSprinting;
        }

        private void OnLocalIsStrafingChanged(bool isStrafing)
        {
            _isStrafing.Value = isStrafing;
        }
        #endregion Animator - Actions

        #region Hand - Actions
        private void OnLocalIsHandVisibleChanged(bool isHandVisible)
        {
            _isHandVisible.Value = isHandVisible;
        }
        #endregion Hand - Actions
        #endregion Local - Actions

        public void DeleteLocal()
        {
            if (IsSpawnedLocal == false) { return; }

            UnsubscribeFromLocalNpc();
            Destroy(_local.gameObject);

            _position.Value = Vector3.zero;
            _rotation.Value = Quaternion.identity;

            DeleteRemoteServerRpc();
        }
        #endregion Local

        #region Remote
        [ServerRpc(RequireOwnership = false)]
        public void SpawnRemoteServerRpc()
        {
            SpawnRemoteClientRpc();
        }

        [ClientRpc]
        public void SpawnRemoteClientRpc()
        {
            SpawnRemote();
        }

        public void SpawnRemote()
        {
            if (IsSpawnedRemote) { return; }
            if (IsOwner) { return; }

            Vector3 position = _position.Value;
            Quaternion rotation = _rotation.Value;
            _remote = Instantiate(_remotePrefab, position, rotation).GetComponent<RemotePlayer>();
            SubscribeToNetworkNpc();
            _remote.SubscribeToNetworkPlayer(this);
        }

        private bool _subscribedToNetwork = false;
        private void SubscribeToNetworkNpc()
        {
            if (_subscribedToNetwork) { return; }

            _position.OnValueChanged += OnNetworkPlayerPositionChanged;
            _rotation.OnValueChanged += OnNetworkPlayerRotationChanged;

            _inputHorizontal.OnValueChanged += OnNetworkInputHorizontalChanged;
            _inputVertical.OnValueChanged += OnNetworkInputVerticalChanged;
            _inputMagnitude.OnValueChanged += OnNetworkInputMagnitudeChanged;
            _groundDistance.OnValueChanged += OnNetworkGroundDistanceChanged;

            _isCrouching.OnValueChanged += OnNetworkIsCrouchingChanged;
            _isGrounded.OnValueChanged += OnNetworkIsGroundedChanged;
            _isSprinting.OnValueChanged += OnNetworkIsSprintingChanged;
            _isStrafing.OnValueChanged += OnNetworkIsStrafingChanged;

            _isHandVisible.OnValueChanged += OnNetworkIsHandVisibleChanged;

            _subscribedToNetwork = true;
        }

        private void UnsubscribeFromNetworkNpc()
        {
            if (_subscribedToNetwork == false) { return; }

            _position.OnValueChanged -= OnNetworkPlayerPositionChanged;
            _rotation.OnValueChanged -= OnNetworkPlayerRotationChanged;

            _inputHorizontal.OnValueChanged -= OnNetworkInputHorizontalChanged;
            _inputVertical.OnValueChanged -= OnNetworkInputVerticalChanged;
            _inputMagnitude.OnValueChanged -= OnNetworkInputMagnitudeChanged;
            _groundDistance.OnValueChanged -= OnNetworkGroundDistanceChanged;

            _isCrouching.OnValueChanged -= OnNetworkIsCrouchingChanged;
            _isGrounded.OnValueChanged -= OnNetworkIsGroundedChanged;
            _isSprinting.OnValueChanged -= OnNetworkIsSprintingChanged;
            _isStrafing.OnValueChanged -= OnNetworkIsStrafingChanged;

            _isHandVisible.OnValueChanged -= OnNetworkIsHandVisibleChanged;

            _subscribedToNetwork = false;
        }

        #region Remote - Actions
        #region Transform - Actions
        private void OnNetworkPlayerPositionChanged(Vector3 previousValue, Vector3 newValue)
        {
            PositionChanged?.Invoke(newValue);
        }

        private void OnNetworkPlayerRotationChanged(Quaternion previousValue, Quaternion newValue)
        {
            RotationChanged?.Invoke(newValue);
        }
        #endregion Transform - Actions

        #region Animator - Actions
        private void OnNetworkInputHorizontalChanged(float previousValue, float newValue)
        {
            InputHorizontalChanged?.Invoke(newValue);
        }

        private void OnNetworkInputVerticalChanged(float previousValue, float newValue)
        {
            InputVerticalChanged?.Invoke(newValue);
        }

        private void OnNetworkInputMagnitudeChanged(float previousValue, float newValue)
        {
            InputMagnitudeChanged?.Invoke(newValue);
        }

        private void OnNetworkGroundDistanceChanged(float previousValue, float newValue)
        {
            GroundDistanceChanged?.Invoke(newValue);
        }

        private void OnNetworkIsCrouchingChanged(bool previousValue, bool newValue)
        {
            IsCrouchingChanged?.Invoke(newValue);
        }

        private void OnNetworkIsGroundedChanged(bool previousValue, bool newValue)
        {
            IsGroundedChanged?.Invoke(newValue);
        }

        private void OnNetworkIsSprintingChanged(bool previousValue, bool newValue)
        {
            IsSprintingChanged?.Invoke(newValue);
        }

        private void OnNetworkIsStrafingChanged(bool previousValue, bool newValue)
        {
            IsStrafingChanged?.Invoke(newValue);
        }
        #endregion Animator - Actions

        #region Hand - Actions
        private void OnNetworkIsHandVisibleChanged(bool previousValue, bool newValue)
        {
            IsHandVisibleChanged?.Invoke(newValue);
        }
        #endregion Hand - Actions
        #endregion Remote - Actions

        [ServerRpc(RequireOwnership = false)]
        private void DeleteRemoteServerRpc()
        {
            if (IsServer == false) { return; }

            DeleteRemoteClientRpc();
        }

        [ClientRpc]
        private void DeleteRemoteClientRpc()
        {
            if (IsOwner) { return; }

            DeleteRemote();
        }

        private void DeleteRemote()
        {
            if (IsSpawnedRemote == false) { return; }

            UnsubscribeFromNetworkNpc();
            Destroy(_remote.gameObject);
        }
        #endregion Remote
        #endregion Body
    }
}