using MLAPI;
using System;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject _cameraPrefab = null;
        [SerializeField] private Vector2 _cameraSensitivity = Vector2.one;
        [SerializeField] private float _walkSpeed = 1f;
        [SerializeField] private float _runSpeed = 2f;

        private InputManager _inputs = null;
        private bool _active = false;

        private Transform _camera = null;
        private Transform _body = null;
        private Animator _animator = null;

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
                        _inputs = new InputManager();

                        _inputs.Game.Move.performed += Move;
                        _inputs.Game.Move.canceled += Move;

                        _inputs.Game.Rotate.performed += Rotate;
                        _inputs.Game.Rotate.canceled += Rotate;

                        _inputs.Game.OpenMenu.performed += OpenMenu;

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

                            _inputs = null;
                        }

                        Direction = Vector2.zero;
                        Delta = Vector2.zero;
                    }
                }
            }
        }

        private void Start()
        {
            _body = LevelManager.Singleton.RandomBody.transform;
            if (_body.TryGetComponent(out Civilian civilian))
            {
                civilian.Active = false;
                civilian.enabled = false;
            }
            if (_body.TryGetComponent(out Animator animator))
            {
                _animator = animator;
            }

            _camera = Instantiate(_cameraPrefab, _body).transform;
            _camera.localPosition = new Vector3(0f, 1.8f, 0f);
            _camera.localEulerAngles = new Vector3(0f, 0f, 0f);
            Cursor.lockState = CursorLockMode.Locked;

            Active = true;

            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }

        private void OnDestroy()
        {
            Active = false;

            if (_body != null && _body.TryGetComponent(out Civilian civilian))
            {
                civilian.enabled = true;
                civilian.Active = true;
            }

            if (NetworkManager.Singleton == null) { return; }

            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                if (_camera != null)
                {
                    Destroy(_camera.gameObject);
                    _camera = null;
                }
                if (_body != null)
                {
                    if (_body.TryGetComponent(out Civilian civilian))
                    {
                        civilian.enabled = true;
                        civilian.Active = true;
                    }
                    _body = null;
                }

                Active = false;
            }
        }

        public Vector2 Direction { get; private set; } = Vector3.zero;
        public Vector2 Delta { get; private set; } = Vector3.zero;

        private void Update()
        {
            if (_body != null)
            {
                Vector3 speed = (_body.forward * Direction.y + _body.right * Direction.x) * _walkSpeed;
                if (Direction != Vector2.zero)
                {
                    _body.position += speed * Time.deltaTime;
                }
                if (_animator != null)
                {
                    _animator.SetFloat("xSpeed", speed.x);
                    _animator.SetFloat("zSpeed", speed.z);
                }
            }
        }

        private void Move(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Direction = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
        }

        private void Rotate(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Delta = context.ReadValue<Vector2>();

            if (_body != null && Delta.x != 0f)
            {
                _body.Rotate(Vector3.up, Delta.x * _cameraSensitivity.x * Time.deltaTime);
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
                    break;
                case CursorLockMode.Locked:
                    Cursor.lockState = CursorLockMode.None;
                    break;
                default:
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }
        }
    }
}
