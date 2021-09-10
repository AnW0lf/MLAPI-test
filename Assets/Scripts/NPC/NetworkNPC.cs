﻿using MLAPI;
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

        private NetworkVariableVector3 _position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector3.zero);

        private NetworkVariableQuaternion _rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Quaternion.identity);

        private NetworkVariableVector2 _velocity = new NetworkVariableVector2(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector2.zero);

        public event Action<Vector3> PositionChanged;
        public event Action<Quaternion> RotationChanged;
        public event Action<Vector2> VelocityChanged;

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

        private void SubscribeToLocalNpc()
        {
            if (_local == null) { return; }

            _local.PositionChanged += OnLocalNpcPositionChanged;
            _local.RotationChanged += OnLocalNpcRotationChanged;
            _local.VelocityChanged += OnLocalNpcVelocityChanged;
        }

        private void UnsubscribeFromLocalNpc()
        {
            if (_local == null) { return; }

            _local.PositionChanged -= OnLocalNpcPositionChanged;
            _local.RotationChanged -= OnLocalNpcRotationChanged;
            _local.VelocityChanged -= OnLocalNpcVelocityChanged;
        }

        private void OnLocalNpcPositionChanged(Vector3 position)
        {
            _position.Value = position;
        }

        private void OnLocalNpcRotationChanged(Quaternion rotation)
        {
            _rotation.Value = rotation;
        }

        private void OnLocalNpcVelocityChanged(Vector2 velocity)
        {
            _velocity.Value = velocity;
        }

        public void DeleteLocal()
        {
            if (IsServer == false) { return; }
            if (IsSpawnedLocal == false) { return; }

            UnsubscribeFromLocalNpc();
            Destroy(_local.gameObject);

            _position.Value = Vector3.zero;
            _rotation.Value = Quaternion.identity;
            _velocity.Value = Vector2.zero;

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

        private void SubscribeToNetworkNpc()
        {
            _position.OnValueChanged += OnNetworkNpcPositionChanged;
            _rotation.OnValueChanged += OnNetworkNpcRotationChanged;
            _velocity.OnValueChanged += OnNetworkNpcVelocityChanged;
        }

        private void UnsubscribeFromNetworkNpc()
        {
            _position.OnValueChanged -= OnNetworkNpcPositionChanged;
            _rotation.OnValueChanged -= OnNetworkNpcRotationChanged;
            _velocity.OnValueChanged -= OnNetworkNpcVelocityChanged;
        }

        private void OnNetworkNpcPositionChanged(Vector3 previousValue, Vector3 newValue)
        {
            PositionChanged?.Invoke(newValue);
        }

        private void OnNetworkNpcRotationChanged(Quaternion previousValue, Quaternion newValue)
        {
            RotationChanged?.Invoke(newValue);
        }

        private void OnNetworkNpcVelocityChanged(Vector2 previousValue, Vector2 newValue)
        {
            VelocityChanged?.Invoke(newValue);
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
    }
}