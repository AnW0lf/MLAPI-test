using MLAPI;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.TestLogic
{
    public class Network : NetworkBehaviour
    {
        [SerializeField] private Local _localPrefab = null;
        [SerializeField] private Remote _remotePrefab = null;

        private Local _local = null;
        private Remote _remote = null;

        #region NetworkVariables
        public readonly NetworkVariableULong ID = new NetworkVariableULong(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, ulong.MaxValue);

        public readonly NetworkVariableBool IsLocalSpawned = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        public readonly NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector3.zero);

        public readonly NetworkVariableQuaternion Rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Quaternion.identity);
        #endregion NetworkVariables

        #region OnLocalChanged Actions
        private Action<Vector3> _positionChanged = null;
        private Action<Quaternion> _rotationChanged = null;
        #endregion OnLocalChanged Actions

        public override void NetworkStart()
        {
            InitializeLocalActions();

            if (IsOwner)
            {
                Vector2 circle = Random.insideUnitCircle * 5f;
                Position.Value = new Vector3(circle.x, 0f, circle.y);
                Rotation.Value = Quaternion.identity;

                SpawnLocal(Position.Value, Rotation.Value);
            }
            else
            {
                IsLocalSpawnedChanged = (oldValue, newValue) =>
                {
                    if (newValue) SpawnRemote(Position.Value, Rotation.Value);
                    else DeleteRemote();
                };
                IsLocalSpawned.OnValueChanged += IsLocalSpawnedChanged;

                if (IsLocalSpawned.Value && _remote == null)
                {
                    SpawnRemote(Position.Value, Rotation.Value);
                }
            }
        }

        private void OnDestroy()
        {
            if (IsOwner)
            {
                DeleteLocal();
            }
            else
            {
                DeleteRemote();
            }
        }

        #region Local
        private void SpawnLocal(Vector3 position, Quaternion rotation)
        {
            DeleteLocal();

            _local = Instantiate(_localPrefab, position, rotation);
            _local.NetworkParent = this;
            SubscribeToLocal();
            SetNetworkAsLocal();

            IsLocalSpawned.Value = _local != null;
        }

        public void DeleteLocal()
        {
            if (_local == null) { return; }

            UnsubscribeFromLocal();
            Destroy(_local.gameObject);
            _local = null;

            IsLocalSpawned.Value = _local != null;
        }

        private void InitializeLocalActions()
        {
            _positionChanged = (value) => Position.Value = value;
            _rotationChanged = (value) => Rotation.Value = value;
        }

        private bool _sunscribed2Local = false;
        private void SubscribeToLocal()
        {
            if (_local == null) { return; }
            if (_sunscribed2Local) { return; }

            _local.Position.ValueChanged += _positionChanged;
            _local.Rotation.ValueChanged += _rotationChanged;

            _sunscribed2Local = true;
        }

        private void UnsubscribeFromLocal()
        {
            if (_local == null) { return; }
            if (_sunscribed2Local == false) { return; }

            _local.Position.ValueChanged -= _positionChanged;
            _local.Rotation.ValueChanged -= _rotationChanged;

            _sunscribed2Local = false;
        }

        private void SetNetworkAsLocal()
        {
            if (_local == null) { return; }

            Position.Value = _local.Position.Value;
            Rotation.Value = _local.Rotation.Value;
        }
        #endregion Local

        #region Remote
        private void SpawnRemote(Vector3 position, Quaternion rotation)
        {
            DeleteRemote();

            _remote = Instantiate(_remotePrefab, position, rotation);
            _remote.NetworkParent = this;
        }

        private void DeleteRemote()
        {
            if (_remote == null) { return; }

            _remote.NetworkParent = null;
            Destroy(_remote.gameObject);
            _remote = null;
        }

        private NetworkVariableBool.OnValueChangedDelegate IsLocalSpawnedChanged;
        #endregion Remote
    }
}