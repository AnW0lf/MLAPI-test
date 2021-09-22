using System;
using UnityEngine;

namespace Assets.Scripts.TestLogic
{
    public class Local : MonoBehaviour
    {
        [Header("Update variables")]
        [SerializeField] private bool _isOnUndate = true;
        [SerializeField] [Range(1, 60)] private int _updateRate = 20;
        private float _delay = 0.05f;
        private float _timer = 0f;

        public Architector Architector { get; set; } = null;

        /// <summary>
        /// Инициализирует необходимые компоненты объекта
        /// </summary>
        protected void Initialize()
        {
            _delay = 1f / _updateRate;
            InitializeVariables();
        }

        /// <summary>
        /// Инициализирует Local переменные
        /// Необходимо переопределить
        /// </summary>
        protected virtual void InitializeVariables() { }

        /// <summary>
        /// Обновляет состояния Local переменных
        /// Необходимо переопределить
        /// </summary>
        protected virtual void UpdateVariables() { }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (_isOnUndate)
            {
                _timer += Time.deltaTime;
                if (_timer > _delay)
                {
                    _timer = 0f;
                    UpdateVariables();
                }
            }
        }
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

    public class LocalString
    {
        private string _old;

        public event Action<string> ValueChanged;

        public LocalString() : this(string.Empty) { }

        public LocalString(string value)
        {
            _old = value;
            ValueChanged?.Invoke(value);
        }

        public string Value
        {
            get => _old;
            set
            {
                if (_old != value)
                {
                    _old = value;
                    ValueChanged?.Invoke(value);
                }
            }
        }
    }

    public class LocalInt
    {
        private int _old;
        private readonly int _offset;

        public event Action<int> ValueChanged;

        public LocalInt() : this(0, 0) { }

        public LocalInt(int value, int offset)
        {
            _old = value;
            _offset = offset;
            ValueChanged?.Invoke(value);
        }

        public int Value
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
    #endregion Local Variables
}