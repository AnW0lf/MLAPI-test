using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Physics components")]
        [SerializeField] private CapsuleCollider _collider = null;
        [SerializeField] private Rigidbody _rigidbody = null;
        [Header("Speeds")]
        [SerializeField] private float _walkSpeed = 2f;
        [SerializeField] private float _runSpeed = 5f;
        [SerializeField] private float _squatWalkSpeed = 1f;
        [SerializeField] private float _squatRunSpeed = 3f;
        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 1.2f;
        [Header("Camera")]
        [SerializeField] private Transform _camera = null;
        [SerializeField] private Vector2 _cameraSensitivity = Vector2.zero;
        [Header("Squat")]
        [SerializeField] private float _fullHeight = 1.7f;
        [SerializeField] private float _squatHeight = 0.9f;

        private float _speed = 0f;

        public Vector2 MoveDirection { get; set; } = Vector2.zero;

        private bool _runMode = false;
        private bool _squatMode = false;
        public bool RunMode
        {
            get => _runMode;
            set
            {
                _runMode = value;
                UpdateSpeed();
            }
        }
        public bool SquatMode
        {
            get => _squatMode;
            set
            {
                _squatMode = value;
                _collider.height = _squatMode ? _squatHeight : _fullHeight;
                UpdateSpeed();
            }
        }

        private Vector3 MoveOffset => (transform.forward * MoveDirection.y + transform.right * MoveDirection.x).normalized * _speed;

        public bool IsGrounded
        {
            get
            {
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.1f, Vector3.one);
                return hits.Any((hit) => hit.transform.CompareTag("Ground"));
            }
        }

        private void FixedUpdate()
        {
            if (MoveDirection != Vector2.zero)
            {
                transform.position += MoveOffset * Time.fixedDeltaTime;
            }
        }

        public void Rotate(Vector2 rotationOffset)
        {
            if (rotationOffset.x != 0f)
            {
                transform.Rotate(Vector3.up, rotationOffset.x * _cameraSensitivity.x * Time.deltaTime);
            }
            if (rotationOffset.y != 0f)
            {
                Vector3 euler = _camera.localEulerAngles + Vector3.left * rotationOffset.y * _cameraSensitivity.y * Time.deltaTime;
                euler.y = 0f;
                euler.z = 0f;
                if (euler.x > 180f) euler.x -= 360f;
                if (euler.x < -180f) euler.x += 360f;
                euler.x = Mathf.Clamp(euler.x, -80f, 80f);
                _camera.localRotation = Quaternion.Euler(euler);
            }
        }

        public void Jump()
        {
            if (IsGrounded == false) { return; }
            if (Mathf.Abs(_rigidbody.velocity.y) > 2f) { return; }

            float jumpSpeed = Mathf.Sqrt(2f * _jumpHeight * Physics.gravity.magnitude);

            _rigidbody.velocity += transform.up * jumpSpeed;
        }

        private void UpdateSpeed()
        {
            _speed = RunMode ? (SquatMode ? _squatRunSpeed : _squatWalkSpeed) : (SquatMode ? _runSpeed : _walkSpeed);
        }
    }
}