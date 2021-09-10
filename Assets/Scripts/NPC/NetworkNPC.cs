using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class NetworkNPC : NetworkBehaviour
    {
        [SerializeField] private GameObject _localPrefab = null;
        [SerializeField] private GameObject _remotePrefab = null;

        private LocalNPC _local = null;
        private RemoteNPC _remote = null;

        private NetworkVariableULong _npcId = new NetworkVariableULong(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, ulong.MaxValue);

        #region NetworkVariables
        #region Transform
        private readonly NetworkVariableVector3 _position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector3.zero);
        public event Action<Vector3> PositionChanged;

        private readonly NetworkVariableQuaternion _rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Quaternion.identity);
        public event Action<Quaternion> RotationChanged;
        #endregion Transform

        #region Animator
        private readonly NetworkVariableFloat _inputHorizontal = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);
        public event Action<float> InputHorizontalChanged;

        private readonly NetworkVariableFloat _inputVertical = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);
        public event Action<float> InputVerticalChanged;

        private readonly NetworkVariableFloat _inputMagnitude = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);
        public event Action<float> InputMagnitudeChanged;

        private readonly NetworkVariableBool _isGrounded = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsGroundedChanged;

        private readonly NetworkVariableBool _isStrafing = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsStrafingChanged;

        private readonly NetworkVariableBool _isSprinting = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsSprintingChanged;

        private readonly NetworkVariableBool _isCrouching = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        public event Action<bool> IsCrouchingChanged;

        private readonly NetworkVariableFloat _groundDistance = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);
        public event Action<float> GroundDistanceChanged;
        #endregion Animator

        #region Skin
        private readonly NetworkVariableInt _skinIndex = new NetworkVariableInt(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, -1);
        public event Action<int> SkinIndexChanged;
        public int SkinIndex => _skinIndex.Value;
        #endregion Skin
        #endregion NetworkVariables

        public ulong NpcId
        {
            get => _npcId.Value;
            set
            {
                if (IsServer)
                {
                    _npcId.Value = value;
                }
            }
        }

        public Transform Body => IsServer ? _local.transform : _remote.transform;

        public bool IsSpawned => IsSpawnedLocal || IsSpawnedRemote;
        public bool IsSpawnedLocal => _local != null;
        public bool IsSpawnedRemote => _remote != null;

        private void OnDestroy()
        {
            DeleteLocal();
            DeleteRemote();
        }

        #region Local
        public void SpawnLocal(Vector3 position, Quaternion rotation)
        {
            if (IsServer == false) { return; }
            if (IsSpawned) { return; }

            _local = Instantiate(_localPrefab, position, rotation).GetComponent<LocalNPC>();
            _local.NetworkParent = this;
            _position.Value = position;
            _rotation.Value = rotation;
            SubscribeToLocalNpc();

            SpawnRemoteClientRpc();
        }

        public void DeleteLocal()
        {
            if (IsServer == false) { return; }
            if (IsSpawnedLocal == false) { return; }

            UnsubscribeFromLocalNpc();
            Destroy(_local.gameObject);

            _position.Value = Vector3.zero;
            _rotation.Value = Quaternion.identity;

            DeleteRemoteServerRpc();
        }
        #endregion Local

        #region Remote
        [ClientRpc]
        private void SpawnRemoteClientRpc()
        {
            if (IsServer) { return; }

            SpawnRemote();
        }

        private void SpawnRemote()
        {
            if (IsSpawnedRemote) { return; }

            Vector3 position = _position.Value;
            Quaternion rotation = _rotation.Value;
            _remote = Instantiate(_remotePrefab, position, rotation).GetComponent<RemoteNPC>();
            SubscribeToNetworkNpc();
            _remote.SubscribeToNetworkNpc(this);
        }

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
            if (IsServer) { return; }
            if (IsSpawnedRemote == false) { return; }

            UnsubscribeFromNetworkNpc();
            Destroy(_remote.gameObject);
        }
        #endregion Remote

        #region Actions

        #region Local
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

            _local.SkinIndexChanged += OnSkinIndexChanged;

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

            _local.SkinIndexChanged -= OnSkinIndexChanged;

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

        #region Skin - Actions
        private void OnSkinIndexChanged(int skinIndex)
        {
            _skinIndex.Value = skinIndex;
        }
        #endregion Skin - Actions
        #endregion Local - Actions
        #endregion Local

        #region Remote
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

            _skinIndex.OnValueChanged += OnNetworkSkinIndexChanged;

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

            _skinIndex.OnValueChanged -= OnNetworkSkinIndexChanged;

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

        #region Skin - Actions
        private void OnNetworkSkinIndexChanged(int previousValue, int newValue)
        {
            SkinIndexChanged?.Invoke(newValue);
        }
        #endregion Skin - Actions
        #endregion Remote - Actions
        #endregion Remote
        #endregion Actions
    }
}