using System;
using UnityEngine;
using MLAPI.NetworkVariable;

namespace Assets.Scripts.TestLogic
{
    public class Remote : MonoBehaviour
    {
        [SerializeField] private Transform _transform = null;
        //[SerializeField] private Animator _animator = null;
        [Header("Maximum offset")]
        [SerializeField] private float _positionOffset = 1f;
        [SerializeField] private float _rotationOffset = 30f;
        [Space(25)]
        [SerializeField] private float _smoothness = 3f;

        private RemoteVector3 _position = null;
        private RemoteQuaternion _rotation = null;


        private NetworkVariableVector3.OnValueChangedDelegate _positionChanged;
        private NetworkVariableQuaternion.OnValueChangedDelegate _rotationChanged;

        private Network _networkParent = null;
        public Network NetworkParent
        {
            get => _networkParent;
            set
            {
                if (_networkParent != null)
                {
                    UnsubscribeFromNetwork();
                }

                _networkParent = value;

                if (_networkParent != null)
                {
                    SubscribeToNetwork();
                }
            }
        }

        private void InitializeRemoteVariables()
        {
            _position = new RemoteVector3(Vector3.zero, _positionOffset);
            _position.IsEquals = (target) => Vector3.Distance(target, _transform.position) == 0f;
            _position.IsOverOffset = (target, offset) => Vector3.Distance(target, _transform.position) > offset;
            _position.SetValue += (value) => _transform.position = value;
            _position.UpdateValue += (value) => _transform.position = Vector3.Lerp(_transform.position, value, _smoothness * Time.deltaTime);

            _rotation = new RemoteQuaternion(Quaternion.identity, _rotationOffset);
            _rotation.IsEquals = (target) => Quaternion.Angle(target, _transform.rotation) == 0f;
            _rotation.IsOverOffset = (target, offset) => Quaternion.Angle(target, _transform.rotation) > offset;
            _rotation.SetValue += (value) => _transform.rotation = value;
            _rotation.UpdateValue += (value) => _transform.rotation = Quaternion.Lerp(_transform.rotation, value, _smoothness * Time.deltaTime);
        }

        private void InitializeNetworkActions()
        {
            _positionChanged = (oldValue, newValue) => _position.Target = newValue;
            _rotationChanged = (oldValue, newValue) => _rotation.Target = newValue;
        }

        private void Awake()
        {
            InitializeRemoteVariables();
            InitializeNetworkActions();
        }

        private void Update()
        {
            _position.Update();
            _rotation.Update();
        }

        private void OnDestroy()
        {
            UnsubscribeFromNetwork();
            NetworkParent = null;
        }

        private bool _subscribedToNetwork = false;
        private void SubscribeToNetwork()
        {
            if (_networkParent == null) { return; }
            if (_subscribedToNetwork) { return; }

            NetworkParent.Position.OnValueChanged += _positionChanged;
            NetworkParent.Rotation.OnValueChanged += _rotationChanged;

            _subscribedToNetwork = true;
        }

        private void UnsubscribeFromNetwork()
        {
            if (_networkParent == null) { return; }
            if (_subscribedToNetwork == false) { return; }

            NetworkParent.Position.OnValueChanged -= _positionChanged;
            NetworkParent.Rotation.OnValueChanged -= _rotationChanged;

            _subscribedToNetwork = false;
        }
    }

    #region Remote Variables
    class RemoteBool
    {
        private bool _target;

        public RemoteBool() : this(false) { }

        public RemoteBool(bool target)
        {
            _target = target;
            UpdateValue?.Invoke(_target);
        }

        public event Action<bool> UpdateValue;
        public Func<bool, bool> IsEquals;

        public bool Target
        {
            get => _target;
            set
            {
                _target = value;
                Update();
            }
        }

        public void Update()
        {
            if (IsEquals(_target)) { return; }
            UpdateValue?.Invoke(_target);
        }
    }

    class RemoteFloat
    {
        private float _target;
        private float _offset;

        public RemoteFloat() : this(0f, 0f) { }

        public RemoteFloat(float target, float offset)
        {
            _target = target;
            _offset = offset;
            UpdateValue?.Invoke(_target);
        }

        public event Action<float> UpdateValue;
        public event Action<float> SetValue;
        public Func<float, bool> IsEquals;
        public Func<float, float, bool> IsOverOffset;

        public float Target
        {
            get => _target;
            set
            {
                _target = value;
                Update();
            }
        }

        public void Update()
        {
            if (IsEquals(_target)) { return; }

            if (IsOverOffset(_target, _offset))
            {
                SetValue.Invoke(_target);
            }
            else
            {
                UpdateValue?.Invoke(_target);
            }
        }
    }

    class RemoteVector2
    {
        private Vector2 _target;
        private float _offset;

        public RemoteVector2() : this(Vector2.zero, 0f) { }

        public RemoteVector2(Vector2 target, float offset)
        {
            _target = target;
            _offset = offset;
            UpdateValue?.Invoke(_target);
        }

        public event Action<Vector2> UpdateValue;
        public event Action<Vector2> SetValue;
        public Func<Vector2, bool> IsEquals;
        public Func<Vector2, float, bool> IsOverOffset;

        public Vector2 Target
        {
            get => _target;
            set
            {
                _target = value;
                Update();
            }
        }

        public void Update()
        {
            if (IsEquals(_target)) { return; }

            if (IsOverOffset(_target, _offset))
            {
                SetValue.Invoke(_target);
            }
            else
            {
                UpdateValue?.Invoke(_target);
            }
        }
    }

    class RemoteVector3
    {
        private Vector3 _target;
        private float _offset;

        public RemoteVector3() : this(Vector3.zero, 0f) { }

        public RemoteVector3(Vector3 target, float offset)
        {
            _target = target;
            _offset = offset;
            UpdateValue?.Invoke(_target);
        }

        public event Action<Vector3> UpdateValue;
        public event Action<Vector3> SetValue;
        public Func<Vector3, bool> IsEquals;
        public Func<Vector3, float, bool> IsOverOffset;

        public Vector3 Target
        {
            get => _target;
            set
            {
                _target = value;
                Update();
            }
        }

        public void Update()
        {
            if (IsEquals(_target)) { return; }

            if (IsOverOffset(_target, _offset))
            {
                SetValue.Invoke(_target);
            }
            else
            {
                UpdateValue?.Invoke(_target);
            }
        }
    }

    class RemoteQuaternion
    {
        private Quaternion _target;
        private float _offset;

        public RemoteQuaternion() : this(Quaternion.identity, 0f) { }

        public RemoteQuaternion(Quaternion target, float offset)
        {
            _target = target;
            _offset = offset;
            UpdateValue?.Invoke(_target);
        }

        public event Action<Quaternion> UpdateValue;
        public event Action<Quaternion> SetValue;
        public Func<Quaternion, bool> IsEquals;
        public Func<Quaternion, float, bool> IsOverOffset;

        public Quaternion Target
        {
            get => _target;
            set
            {
                _target = value;
                Update();
            }
        }

        public void Update()
        {
            if (IsEquals(_target)) { return; }

            if (IsOverOffset(_target, _offset))
            {
                SetValue.Invoke(_target);
            }
            else
            {
                UpdateValue?.Invoke(_target);
            }
        }
    }
    #endregion Remote Variables
}