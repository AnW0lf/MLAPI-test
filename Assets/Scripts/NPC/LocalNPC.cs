using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class LocalNPC : MonoBehaviour
    {
        [SerializeField] private Transform _body = null;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Rigidbody _rigidbody = null;
        [Header("Minimum difference")]
        [SerializeField] private float _minPositionStep = 0.2f;
        [SerializeField] private float _minRotationStep = 1.5f;
        [SerializeField] private float _minVelocityStep = 0.1f;

        private Vector3 _oldPosition = Vector3.zero;
        private Quaternion _oldRotation = Quaternion.identity;
        private Vector2 _oldVelocity = Vector2.zero;

        private Vector3 _position => _body.position;
        private Quaternion _rotation => _body.rotation;
        private Vector2 _velocity
        {
            get
            {
                float xx = Vector3.Project(_rigidbody.velocity, _body.right).magnitude;
                float yy = Vector3.Project(_rigidbody.velocity, _body.forward).magnitude;
                return new Vector2(xx, yy);
            }
        }

        public event Action<Vector3> PositionChanged;
        public event Action<Quaternion> RotationChanged;
        public event Action<Vector2> VelocityChanged;

        private void Update()
        {
            if (Vector3.Distance(_oldPosition, _position) > _minPositionStep)
            {
                _oldPosition = _position;
                PositionChanged?.Invoke(_position);
            }

            if (Quaternion.Angle(_oldRotation, _rotation) > _minRotationStep)
            {
                _oldRotation = _rotation;
                RotationChanged?.Invoke(_rotation);
            }

            Vector2 velocity = _velocity;
            _animator.SetFloat("velocityX", velocity.x);
            _animator.SetFloat("velocityY", velocity.y);

            if (Vector2.Distance(_oldVelocity, velocity) > _minVelocityStep)
            {
                _oldVelocity = velocity;
                VelocityChanged?.Invoke(velocity);
            }
        }
    }
}