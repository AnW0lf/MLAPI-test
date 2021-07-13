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
                    _body = null;
                }

                Active = false;
            }
        }

        public Vector2 Direction { get; private set; } = Vector3.zero;
        public Vector2 Delta { get; private set; } = Vector3.zero;

        private void Update()
        {
            if (_body != null && Direction != Vector2.zero)
            {
                _body.position += (_body.forward * Direction.y + _body.right * Direction.x) * _walkSpeed * Time.deltaTime;
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
                if (euler.y > 180f) euler.y -= 360f;
                euler.y = Mathf.Clamp(euler.y, -80f, 80f);
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
