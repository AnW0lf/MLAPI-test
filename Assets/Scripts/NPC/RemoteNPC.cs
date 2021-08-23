using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class RemoteNPC : MonoBehaviour
    {
        [SerializeField] private Transform _body = null;
        [SerializeField] private Animator _animator = null;
        [Header("Smoothness")]
        [SerializeField] private float _positionSmoothness = 3f;
        [SerializeField] private float _rotationSmoothness = 3f;
        [SerializeField] private float _velocitySmoothness = 3f;

        private Vector3 _targetPosition = Vector3.zero;
        private Quaternion _targetRotation = Quaternion.identity;
        private Vector2 _targetVelocity = Vector2.zero;

        private Vector3 _position
        {
            get => _body.position;
            set => _body.position = value;
        }

        private Quaternion _rotation
        {
            get => _body.rotation;
            set => _body.rotation = value;
        }

        private Vector2 _velocityCash = Vector2.zero;
        private Vector2 _velocity
        {
            get => _velocityCash;
            set
            {
                _velocityCash = value;

                _animator.SetFloat("velocityX", _velocityCash.x);
                _animator.SetFloat("velocityY", _velocityCash.y);
            }
        }

        private NetworkNPC _netNpc = null;

        public void SubscribeToNetworkNpc(NetworkNPC netNpc)
        {
            _netNpc = netNpc;

            if (_netNpc == null) { return; }

            _netNpc.PositionChanged += SetPosition;
            _netNpc.RotationChanged += SetRotation;
            _netNpc.VelocityChanged += SetVelocity;
        }

        private void UnsubscribeFromNetworkNpc()
        {
            if (_netNpc == null) { return; }

            _netNpc.PositionChanged -= SetPosition;
            _netNpc.RotationChanged -= SetRotation;
            _netNpc.VelocityChanged -= SetVelocity;
        }

        private void SetPosition(Vector3 position)
        {
            _targetPosition = position;
        }

        private void SetRotation(Quaternion rotation)
        {
            _targetRotation = rotation;
        }

        private void SetVelocity(Vector2 velocity)
        {
            _targetVelocity = velocity;
        }

        private void Update()
        {
            _position = Vector3.Lerp(_position, _targetPosition, _positionSmoothness * Time.deltaTime);
            _rotation = Quaternion.Lerp(_rotation, _targetRotation, _rotationSmoothness * Time.deltaTime);
            _velocity = Vector2.Lerp(_velocity, _targetVelocity, _velocitySmoothness * Time.deltaTime);
        }

        private void OnDestroy()
        {
            UnsubscribeFromNetworkNpc();
        }
    }
}