using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class RemotePlayer : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [Header("Minimum changes")]
        [SerializeField] private float _minPositionStep = 0.15f;
        [SerializeField] private float _minRotationStep = 1.5f;
        [SerializeField] private float _minVelocityStep = 0.1f;
        [Header("Smoothness")]
        [SerializeField] private float _positionSmoothness = 3f;
        [SerializeField] private float _rotationSmoothness = 3f;
        [SerializeField] private float _velocitySmoothness = 3f;

        private NetworkLocalPlayer _networkPlayer = null;

        private Vector3 _targetPosition = Vector3.zero;
        private Quaternion _targetRotation = Quaternion.identity;
        private Vector2 _targetVelocity = Vector2.zero;
        private Vector2 _velocity = Vector2.zero;

        public Vector2 AnimatorVelocity
        {
            get
            {
                Vector2 velocity = Vector2.zero;
                velocity.x = _animator.GetFloat("velocityX");
                velocity.y = _animator.GetFloat("velocityY");
                return velocity;
            }
            private set
            {
                _animator.SetFloat("velocityX", value.x);
                _animator.SetFloat("velocityY", value.y);
            }
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _positionSmoothness * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, _rotationSmoothness * Time.deltaTime);
            _velocity.x = Vector3.Project(_targetVelocity, transform.right).magnitude;
            _velocity.y = Vector3.Project(_targetVelocity, transform.forward).magnitude;
            AnimatorVelocity = Vector2.Lerp(AnimatorVelocity, _velocity, _velocitySmoothness * Time.deltaTime);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void ConnectToNetworkPlayer(NetworkLocalPlayer netPlayer)
        {
            _networkPlayer = netPlayer;
            Subscribe();
        }

        private void Subscribe()
        {
            if (_networkPlayer == null) { return; }

            _networkPlayer.OnPositionChanged += UpdatePosition;
            _networkPlayer.OnRotationChanged += UpdateRotation;
            _networkPlayer.OnVelocityChanged += UpdateAnimator;
        }
        private void Unsubscribe()
        {
            if (_networkPlayer == null) { return; }

            _networkPlayer.OnPositionChanged -= UpdatePosition;
            _networkPlayer.OnRotationChanged -= UpdateRotation;
            _networkPlayer.OnVelocityChanged -= UpdateAnimator;
        }


        private void UpdatePosition(Vector3 position)
        {
            if(Vector3.Distance(_targetPosition, position) > _minPositionStep)
            {
                _targetPosition = position;
            }
        }

        private void UpdateRotation(Quaternion rotation)
        {
            if (Quaternion.Angle(_targetRotation, rotation) > _minRotationStep)
            {
                _targetRotation = rotation;
            }
        }

        private void UpdateAnimator(Vector2 velocity)
        {
            if (velocity == Vector2.zero)
            {
                _targetVelocity = velocity;
            }
            else if (Vector2.Distance(_targetVelocity, velocity) > _minVelocityStep)
            {
                _targetVelocity = velocity;
            }
        }
    }
}