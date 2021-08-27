using UnityEngine;

namespace Assets.Scripts.Player
{
    public class RemotePlayer : MonoBehaviour
    {
        [SerializeField] private Transform _body = null;
        [SerializeField] private Animator _animator = null;
        [Header("Smoothness")]
        [SerializeField] private float _positionSmoothness = 3f;
        [SerializeField] private float _rotationSmoothness = 3f;
        [SerializeField] private float _velocitySmoothness = 3f;
        [Header("Maximum difference to interpolate")]
        [SerializeField] private float _maxPositionStep = 1.5f;
        [SerializeField] private float _maxRotationStep = 30f;
        [SerializeField] private float _maxVelocityStep = 1f;

        private Vector3 _targetPosition = Vector3.zero;
        private Quaternion _targetRotation = Quaternion.identity;
        private Vector2 _targetVelocity = Vector2.zero;

        public Vector3 Position
        {
            get => _body.position;
            private set => _body.position = value;
        }

        public Quaternion Rotation
        {
            get => _body.rotation;
            private set => _body.rotation = value;
        }

        private Vector2 _velocityCash = Vector2.zero;
        public Vector2 Velocity
        {
            get => _velocityCash;
            private set
            {
                _velocityCash = value;

                _animator.SetFloat("velocityX", _velocityCash.x);
                _animator.SetFloat("velocityY", _velocityCash.y);
                _animator.SetFloat("velocityMagnitude", _velocityCash.magnitude);
            }
        }

        private NetworkLocalPlayer _networkPlayer = null;
        public NetworkLocalPlayer NetworkParent => _networkPlayer;

        public void SubscribeToNetworkPlayer(NetworkLocalPlayer networkPlayer)
        {
            _networkPlayer = networkPlayer;

            if (_networkPlayer == null) { return; }

            _networkPlayer.PositionChanged += SetTargetPosition;
            _networkPlayer.RotationChanged += SetTargetRotation;
            _networkPlayer.VelocityChanged += SetTargetVelocity;
            _networkPlayer.VelocityChanged += SetTargetVelocity;
        }

        private void UnsubscribeFromNetworkPlayer()
        {
            if (_networkPlayer == null) { return; }

            _networkPlayer.PositionChanged -= SetTargetPosition;
            _networkPlayer.RotationChanged -= SetTargetRotation;
            _networkPlayer.VelocityChanged -= SetTargetVelocity;
        }

        private void SetTargetPosition(Vector3 position)
        {
            _targetPosition = position;
        }

        private void SetTargetRotation(Quaternion rotation)
        {
            _targetRotation = rotation;
        }

        private void SetTargetVelocity(Vector2 velocity)
        {
            _targetVelocity = velocity;
        }

        private void Update()
        {
            if (Vector3.Distance(Position, _targetPosition) > _maxPositionStep)
            {
                Position = _targetPosition;
            }
            else
            {
                Position = Vector3.Lerp(Position, _targetPosition, _positionSmoothness * Time.deltaTime);
            }

            if (Quaternion.Angle(Rotation, _targetRotation) > _maxRotationStep)
            {
                Rotation = _targetRotation;
            }
            else
            {
                Rotation = Quaternion.Lerp(Rotation, _targetRotation, _rotationSmoothness * Time.deltaTime);
            }

            if (Vector2.Distance(Velocity, _targetVelocity) > _maxVelocityStep)
            {
                Velocity = _targetVelocity;
            }
            else
            {
                Velocity = Vector2.Lerp(Velocity, _targetVelocity, _velocitySmoothness * Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromNetworkPlayer();
        }
    }
}