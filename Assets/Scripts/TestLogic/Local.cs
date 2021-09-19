using System;
using UnityEngine;

namespace Assets.Scripts.TestLogic
{
    public class Local : MonoBehaviour
    {
        [SerializeField] private Transform _transform = null;
        //[SerializeField] private Animator _animator = null;
        [Header("Minimum offset")]
        [SerializeField] private float _positionOffset = 0.15f;
        [SerializeField] private float _rotationOffset = 1.5f;

        public LocalVector3 Position { get; private set; }
        public LocalQuaternion Rotation { get; private set; }

        public Network NetworkParent { get; set; } = null;

        private void InitializeVariables()
        {
            Position = new LocalVector3(_transform.position, _positionOffset);
            Rotation = new LocalQuaternion(_transform.rotation, _rotationOffset);
        }

        private void UpdateVariables()
        {
            Position.Value = _transform.position;
            Rotation.Value = _transform.rotation;
        }

        private void Awake()
        {
            InitializeVariables();
        }

        private void Update()
        {
            UpdateVariables();
        }
    }

    public static class AnimatorParameters
    {
        public static int InputHorizontal = Animator.StringToHash("InputHorizontal");
        public static int InputVertical = Animator.StringToHash("InputVertical");
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static int GroundDistance = Animator.StringToHash("GroundDistance");
        public static int IsCrouching = Animator.StringToHash("IsCrouching");
        public static int IsGrounded = Animator.StringToHash("IsGrounded");
        public static int IsStrafing = Animator.StringToHash("IsStrafing");
        public static int IsSprinting = Animator.StringToHash("IsSprinting");
    }

    #region Local Variables
    public class LocalBool
    {
        private bool _old;

        public event Action<bool> ValueChanged;

        public LocalBool() : this(false) { }

        public LocalBool(bool value)
        {
            _old = value;
            ValueChanged?.Invoke(value);
        }

        public bool Value
        {
            get => _old;
            set
            {
                if (value != _old)
                {
                    _old = value;
                    ValueChanged?.Invoke(value);
                }
            }
        }
    }

    public class LocalFloat
    {
        private float _old;
        private readonly float _offset;

        public event Action<float> ValueChanged;

        public LocalFloat() : this(0f, 0f) { }

        public LocalFloat(float value, float offset)
        {
            _old = value;
            _offset = offset;
            ValueChanged?.Invoke(value);
        }

        public float Value
        {
            get => _old;
            set
            {
                if (Mathf.Abs(value - _old) > _offset)
                {
                    _old = value;
                    ValueChanged?.Invoke(value);
                }
            }
        }
    }

    public class LocalVector2
    {
        private Vector2 _old;
        private readonly float _offset;

        public event Action<Vector2> ValueChanged;

        public LocalVector2() : this(Vector2.zero, 0f) { }

        public LocalVector2(Vector2 value, float offset)
        {
            _old = value;
            _offset = offset;
            ValueChanged?.Invoke(value);
        }

        public Vector2 Value
        {
            get => _old;
            set
            {
                if (Vector2.Distance(value, _old) > _offset)
                {
                    _old = value;
                    ValueChanged?.Invoke(value);
                }
            }
        }
    }

    public class LocalVector3
    {
        private Vector3 _old;
        private readonly float _offset;

        public event Action<Vector3> ValueChanged;

        public LocalVector3() : this(Vector3.zero, 0f) { }

        public LocalVector3(Vector3 value, float offset)
        {
            _old = value;
            _offset = offset;
            ValueChanged?.Invoke(value);
        }

        public Vector3 Value
        {
            get => _old;
            set
            {
                if (Vector3.Distance(value, _old) > _offset)
                {
                    _old = value;
                    ValueChanged?.Invoke(value);
                }
            }
        }
    }

    public class LocalQuaternion
    {
        private Quaternion _old;
        private readonly float _offset;

        public event Action<Quaternion> ValueChanged;

        public LocalQuaternion() : this(Quaternion.identity, 0f) { }

        public LocalQuaternion(Quaternion value, float offset)
        {
            _old = value;
            _offset = offset;
            ValueChanged?.Invoke(value);
        }

        public Quaternion Value
        {
            get => _old;
            set
            {
                if (Quaternion.Angle(value, _old) > _offset)
                {
                    _old = value;
                    ValueChanged?.Invoke(value);
                }
            }
        }
    }
    #endregion Local Variables
}