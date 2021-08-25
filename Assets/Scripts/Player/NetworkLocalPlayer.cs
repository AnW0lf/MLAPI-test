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

        public event Action<Vector3> PositionChanged;
        public event Action<Quaternion> RotationChanged;
        public event Action<Vector2> VelocityChanged;

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

        private void SubscribeToLocalNpc()
        {
            if (IsSpawnedLocal == false) { return; }

            _local.PositionChanged += OnLocalPlayerPositionChanged;
            _local.RotationChanged += OnLocalPlayerRotationChanged;
            _local.VelocityChanged += OnLocalPlayerVelocityChanged;
        }

        private void UnsubscribeFromLocalNpc()
        {
            if (IsSpawnedLocal == false) { return; }

            _local.PositionChanged -= OnLocalPlayerPositionChanged;
            _local.RotationChanged -= OnLocalPlayerRotationChanged;
            _local.VelocityChanged -= OnLocalPlayerVelocityChanged;
        }

        private void OnLocalPlayerPositionChanged(Vector3 position)
        {
            _position.Value = position;
        }

        private void OnLocalPlayerRotationChanged(Quaternion rotation)
        {
            _rotation.Value = rotation;
        }

        private void OnLocalPlayerVelocityChanged(Vector2 velocity)
        {
            _velocity.Value = velocity;
        }

        public void DeleteLocal()
        {
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

        private void SubscribeToNetworkNpc()
        {
            _position.OnValueChanged += OnNetworkPlayerPositionChanged;
            _rotation.OnValueChanged += OnNetworkPlayerRotationChanged;
            _velocity.OnValueChanged += OnNetworkPlayerVelocityChanged;
        }

        private void UnsubscribeFromNetworkNpc()
        {
            _position.OnValueChanged -= OnNetworkPlayerPositionChanged;
            _rotation.OnValueChanged -= OnNetworkPlayerRotationChanged;
            _velocity.OnValueChanged -= OnNetworkPlayerVelocityChanged;
        }

        private void OnNetworkPlayerPositionChanged(Vector3 previousValue, Vector3 newValue)
        {
            PositionChanged?.Invoke(newValue);
        }

        private void OnNetworkPlayerRotationChanged(Quaternion previousValue, Quaternion newValue)
        {
            RotationChanged?.Invoke(newValue);
        }

        private void OnNetworkPlayerVelocityChanged(Vector2 previousValue, Vector2 newValue)
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
            if (IsSpawnedRemote == false) { return; }

            UnsubscribeFromNetworkNpc();
            Destroy(_remote.gameObject);
        }
        #endregion Remote
        #endregion Body
    }
}