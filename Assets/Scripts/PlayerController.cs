using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _camera = null;
        [SerializeField] private Vector2 _cameraSensitivity = Vector2.one;
        [SerializeField] private float _walkSpeed = 2f;
        [SerializeField] private float _runSpeed = 4f;

        private InputManager _inputs = null;
        private bool _active = false;
        private NetworkPlayer _owner = null;

        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    if (_active)
                    {
                        _owner = GetComponent<NetworkPlayer>();

                        _inputs = new InputManager();

                        _inputs.Game.Move.performed += Move;
                        _inputs.Game.Move.canceled += Move;

                        _inputs.Game.Rotate.performed += Rotate;
                        _inputs.Game.Rotate.canceled += Rotate;

                        _inputs.Game.OpenMenu.performed += OpenMenu;

                        _inputs.Game.Weapon.performed += ShowHideWeapon;
                        _inputs.Game.Fire.performed += Fire;

                        _inputs.Game.Enable();

                    }
                    else
                    {
                        if (_inputs != null)
                        {
                            _inputs.Game.Disable();

                            _inputs.Game.Move.performed -= Move;
                            _inputs.Game.Move.canceled -= Move;

                            _inputs.Game.Rotate.performed -= Rotate;
                            _inputs.Game.Rotate.canceled -= Rotate;

                            _inputs.Game.OpenMenu.performed -= OpenMenu;

                            _inputs.Game.Weapon.performed -= ShowHideWeapon;
                            _inputs.Game.Fire.performed -= Fire;

                            _inputs = null;
                        }

                        Direction = Vector2.zero;
                        Delta = Vector2.zero;
                    }
                }
            }
        }

        public Vector2 Direction { get; private set; } = Vector3.zero;
        public Vector2 Delta { get; private set; } = Vector3.zero;

        private Vector3 oldSpeed = Vector3.zero;

        private void Update()
        {
            Vector3 speed = (transform.forward * Direction.y + transform.right * Direction.x) * _walkSpeed;
            if (Direction != Vector2.zero)
            {
                transform.position += speed * Time.deltaTime;
            }
            if (oldSpeed != speed)
            {
                _animator.SetFloat("xSpeed", speed.x);
                _animator.SetFloat("zSpeed", speed.z);
                oldSpeed = speed;
            }
        }

        private void ShowHideWeapon(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _owner.IsWeaponVisible = !_owner.IsWeaponVisible;
        }

        private void Fire(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (Cursor.lockState != CursorLockMode.Locked) { return; }

            _owner.Shoot();
        }

        private void Move(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (Cursor.lockState != CursorLockMode.Locked) { return; }

            Direction = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
        }

        private void Rotate(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (Cursor.lockState != CursorLockMode.Locked) { return; }

            Delta = context.ReadValue<Vector2>();

            if (Delta.x != 0f)
            {
                transform.Rotate(Vector3.up, Delta.x * _cameraSensitivity.x * Time.deltaTime);
            }

            if (_camera != null && Delta.y != 0f)
            {
                Vector3 euler = _camera.localEulerAngles + Vector3.left * Delta.y * _cameraSensitivity.y * Time.deltaTime;
                euler.y = 0f;
                euler.z = 0f;
                if (euler.x > 180f) euler.x -= 360f;
                if (euler.x < -180f) euler.x += 360f;
                euler.x = Mathf.Clamp(euler.x, -80f, 80f);
                _camera.localRotation = Quaternion.Euler(euler);
            }
        }

        private void OpenMenu(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            switch (Cursor.lockState)
            {
                case CursorLockMode.None:
                    Cursor.lockState = CursorLockMode.Locked;
                    Direction = Vector3.zero;
                    Delta = Vector2.zero;
                    break;
                case CursorLockMode.Locked:
                    Cursor.lockState = CursorLockMode.None;
                    break;
                default:
                    Cursor.lockState = CursorLockMode.None;
                    Direction = Vector3.zero;
                    Delta = Vector2.zero;
                    break;
            }
        }
    }
}
