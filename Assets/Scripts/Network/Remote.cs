using System;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class Remote : MonoBehaviour
    {
        private bool _subscribedToArchitector = false;
        private Architector _architector = null;

        /// <summary>
        /// Свойство определяющее логику работы с сылкой на Network объект
        /// Инкапсулирует логику для безопасного изменения данных
        /// </summary>
        public Architector Architector
        {
            get => _architector;
            set
            {
                if (_architector != null)
                {
                    UnsubscribeFromArchitector();
                }

                _architector = value;

                if (_architector != null)
                {
                    SubscribeToArchitector();
                }
            }
        }

        /// <summary>
        /// Инициализирует необходимые компоненты объекта
        /// </summary>
        protected void Initialize()
        {
            InitializeVariables();
            InitializeArchitectorActions();
        }

        /// <summary>
        /// Инициализирует Remote переменные
        /// Необходимо переопределить
        /// </summary>
        protected virtual void InitializeVariables() { }

        /// <summary>
        /// Инициализирует Actions для соответствующих событий NetworkParent
        /// Необходимо переопределить
        /// </summary>
        protected virtual void InitializeArchitectorActions() { }

        /// <summary>
        /// Подписывает объект на события NetworkParent
        /// Содержит проверки корректности операции
        /// Сами подписки необходимо реализовать переопределением метода Subscribe
        /// </summary>
        protected void SubscribeToArchitector()
        {
            if (_architector == null) { return; }
            if (_subscribedToArchitector) { return; }

            Subscribe();

            _subscribedToArchitector = true;
        }

        /// <summary>
        /// Отписывает объект от событий NetworkParent
        /// Содержит проверки корректности операции
        /// Сами отписки необходимо реализовать переопределением метода Unsubscribe
        /// </summary>
        protected void UnsubscribeFromArchitector()
        {
            if (_architector == null) { return; }
            if (_subscribedToArchitector == false) { return; }

            Unsubscribe();

            _subscribedToArchitector = false;
        }

        /// <summary>
        /// Осуществляет подписку объекта на события NetworkParent
        /// Необходимо переопределить в соответствии с требуемыми событиями
        /// Не требует проверок на корректность операций при работе с NetworkParent
        /// </summary>
        protected virtual void Subscribe() { }

        /// <summary>
        /// Осуществляет отписку объекта от событий NetworkParent
        /// Необходимо переопределить в соответствии с требуемыми событиями
        /// Не требует проверок на корректность операций при работе с NetworkParent
        /// </summary>
        protected virtual void Unsubscribe() { }

        /// <summary>
        /// Вызывает метод Update в Remote переменных объекта
        /// Необходимо переопределить
        /// </summary>
        protected virtual void UpdateVariables() { }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            UpdateVariables();
        }

        private void OnDestroy()
        {
            Architector = null;
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
            SetValue?.Invoke(_target);
        }

        public event Action<bool> SetValue;
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
            SetValue?.Invoke(_target);
        }
    }

    class RemoteFloat
    {
        private float _target;
        private readonly float _offset;

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
        private readonly float _offset;

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
        private readonly float _offset;

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
        private readonly float _offset;

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

    class RemoteString
    {
        private string _target;

        public RemoteString() : this(string.Empty) { }

        public RemoteString(string target)
        {
            _target = target;
            SetValue?.Invoke(_target);
        }

        public event Action<string> SetValue;
        public Func<string, bool> IsEquals;

        public string Target
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
            SetValue?.Invoke(_target);
        }
    }

    class RemoteInt
    {
        private int _target;
        private readonly int _offset;

        public RemoteInt() : this(0, 0) { }

        public RemoteInt(int target, int offset)
        {
            _target = target;
            _offset = offset;
            UpdateValue?.Invoke(_target);
        }

        public event Action<int> UpdateValue;
        public event Action<int> SetValue;
        public Func<int, bool> IsEquals;
        public Func<int, int, bool> IsOverOffset;

        public int Target
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