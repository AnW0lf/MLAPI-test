using Assets.Scripts.Lobby;
using MLAPI;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class NetworkActor : NetworkBehaviour
    {
        [SerializeField] private Architector _bodyArchitector = null;
        [SerializeField] private Architector _cardArchitector = null;
        public Architector BodyArchitector => _bodyArchitector;
        public Architector CardArchitector => _cardArchitector;

        public readonly NetworkVariableULong ID = new NetworkVariableULong(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, ulong.MaxValue);

        public override void NetworkStart()
        {
            _bodyArchitector?.Initialize();
            _cardArchitector?.Initialize();

            if (IsOwner)
            {

            }
            else
            {
                if (_bodyArchitector != null)
                {
                    _bodyArchitector.IsLocalSpawned.OnValueChanged += OnIsLocalBodySpawnedChanged;
                    OnIsLocalBodySpawnedChanged(_bodyArchitector.IsLocalSpawned.Value, _bodyArchitector.IsLocalSpawned.Value);
                }

                if (_cardArchitector != null)
                {
                    _cardArchitector.IsLocalSpawned.OnValueChanged += OnIsLocalCardSpawnedChanged;
                    OnIsLocalCardSpawnedChanged(_cardArchitector.IsLocalSpawned.Value, _cardArchitector.IsLocalSpawned.Value);
                }
            }

        }

        private void OnDestroy()
        {
            if (IsOwner)
            {
                _bodyArchitector?.DeleteLocal();
                _cardArchitector?.DeleteLocal();
            }
            else
            {
                _bodyArchitector?.DeleteRemote();
                _cardArchitector?.DeleteRemote();
            }
        }

        private void OnIsLocalBodySpawnedChanged(bool previousValue, bool newValue)
        {
            if (IsOwner) { return; }

            if (newValue)
            {
                _bodyArchitector?.SpawnRemote();
            }
            else
            {
                _bodyArchitector?.DeleteRemote();
            }
        }

        private void OnIsLocalCardSpawnedChanged(bool previousValue, bool newValue)
        {
            if (IsOwner) { return; }

            if (newValue)
            {
                PlayerPanel playerPanel = FindObjectOfType<PlayerPanel>();

                if (playerPanel == null)
                {
                    throw new NullReferenceException("Object with component of type \"PlayerPanel\" not found");
                }

                _cardArchitector?.SpawnRemote(playerPanel.transform);
            }
            else
            {
                _cardArchitector?.DeleteRemote();
            }
        }
    }
}