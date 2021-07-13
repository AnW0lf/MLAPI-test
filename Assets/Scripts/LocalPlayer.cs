using MLAPI;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class LocalPlayer : MonoBehaviour
    {
        [Header("Network")]
        [SerializeField] private NetworkObject _networkObject = null;

        [Header("Camera")]
        [SerializeField] private GameObject _cameraPrefab = null;
        [Range(0f, 100f)]
        [SerializeField] private float _verticalSensitivity = 1f;
        [Range(0f, 100f)]
        [SerializeField] private float _horizontalSensitivity = 1f;

        [Header("Move")]
        [Range(0f, 20f)]
        [SerializeField] private float _walkSpeed = 2f;
        [Range(0f, 20f)]
        [SerializeField] private float _runSpeed = 4f;

        private Camera _camera = null;

        private Vector2 _direction = Vector2.zero;
        public Vector2 Direction
        {
            get => _direction;
            set => _direction = Vector2.ClampMagnitude(value, 1f);
        }
        public Vector2 MouseDelta { get; set; }
        public bool RunMode { get; set; }

        private void OnEnable()
        {
            StartCoroutine(DelayedCall());
        }

        private IEnumerator DelayedCall()
        {
            yield return null;
            yield return null;

            if (_networkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                if (_camera == null)
                {
                    _camera = Instantiate(_cameraPrefab, transform).GetComponent<Camera>();
                }
            }
        }

        private void OnDisable()
        {
            if (_camera != null)
            {
                Destroy(_camera.gameObject);
                _camera = null;
            }
        }

        private void OnDestroy()
        {
            if (_camera != null)
            {
                Destroy(_camera.gameObject);
                _camera = null;
            }
        }

        private void Update()
        {
            if (Direction != Vector2.zero)
            {
                Vector3 dir = transform.forward * Direction.y + transform.right * Direction.x;
                transform.position += dir * (RunMode ? _runSpeed : _walkSpeed) * Time.deltaTime;
            }

            if (MouseDelta.x != 0f)
            {
                Quaternion rotation = Quaternion.Euler(transform.eulerAngles
                    + Vector3.up * MouseDelta.x * _horizontalSensitivity * Time.deltaTime);
                transform.rotation = rotation;
            }

            if (_camera != null)
            {
                if (MouseDelta.y != 0f)
                {
                    Vector3 euler = _camera.transform.localEulerAngles;
                    if (euler.x > 180f) euler.x -= 360f;
                    euler += Vector3.left * MouseDelta.y * _verticalSensitivity * Time.deltaTime;
                    euler.x = Mathf.Clamp(euler.x, -85f, 85f);
                    _camera.transform.localRotation = Quaternion.Euler(euler);
                }
            }

            MouseDelta = Vector2.zero;
        }
    }
}
