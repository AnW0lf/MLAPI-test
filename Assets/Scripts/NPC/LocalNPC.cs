using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC
{
    public class LocalNPC : MonoBehaviour
    {
        [SerializeField] private Transform _body = null;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private NavMeshAgent _agent = null;
        [Header("Minimum difference")]
        [SerializeField] private float _minPositionStep = 0.2f;
        [SerializeField] private float _minRotationStep = 1.5f;
        [SerializeField] private float _minAnimatorFloatStep = 0.1f;
        [Header("Skin")]
        [SerializeField] private GameObject[] _skins = null;

        #region Transform
        private Vector3 _position => _body.position;
        private Vector3 _oldPosition = Vector3.zero;
        public event Action<Vector3> PositionChanged;
        private Quaternion _rotation => _body.rotation;
        private Quaternion _oldRotation = Quaternion.identity;
        public event Action<Quaternion> RotationChanged;
        #endregion Transform

        #region Animator
        private readonly static int n_InputHorizontal = Animator.StringToHash("InputHorizontal");
        private readonly static int n_InputVertical = Animator.StringToHash("InputVertical");
        private readonly static int n_InputMagnitude = Animator.StringToHash("InputMagnitude");
        private readonly static int n_IsGrounded = Animator.StringToHash("IsGrounded");
        private readonly static int n_IsStrafing = Animator.StringToHash("IsStrafing");
        private readonly static int n_IsSprinting = Animator.StringToHash("IsSprinting");
        private readonly static int n_IsCrouching = Animator.StringToHash("IsCrouching");
        private readonly static int n_GroundDistance = Animator.StringToHash("GroundDistance");

        private float _inputHorizontal => _animator.GetFloat(n_InputHorizontal);
        private float _oldInputHorizontal = 0f;
        public event Action<float> InputHorizontalChanged;

        private float _inputVertical => _animator.GetFloat(n_InputVertical);
        private float _oldInputVertical = 0f;
        public event Action<float> InputVerticalChanged;

        private float _inputMagnitude => _animator.GetFloat(n_InputMagnitude);
        private float _oldInputMagnitude = 0f;
        public event Action<float> InputMagnitudeChanged;

        private bool _isGrounded => _animator.GetBool(n_IsGrounded);
        private bool _oldIsGrounded = false;
        public event Action<bool> IsGroundedChanged;

        private bool _isStrafing => _animator.GetBool(n_IsStrafing);
        private bool _oldIsStrafing = false;
        public event Action<bool> IsStrafingChanged;

        private bool _isSprinting => _animator.GetBool(n_IsSprinting);
        private bool _oldIsSprinting = false;
        public event Action<bool> IsSprintingChanged;

        private bool _isCrouching => _animator.GetBool(n_IsCrouching);
        private bool _oldIsCrouching = false;
        public event Action<bool> IsCrouchingChanged;

        private float _groundDistance => _animator.GetFloat(n_GroundDistance);
        private float _oldGroundDistance = 0f;
        public event Action<float> GroundDistanceChanged;
        #endregion Animator

        #region Skin
        public event Action<int> SkinIndexChanged;
        #endregion Skin

        public NetworkNPC NetworkParent { get; set; } = null;

        private void Start()
        {
            // Set random skin
            foreach (var skin in _skins) skin.SetActive(false);
            int skinIndex = UnityEngine.Random.Range(0, _skins.Length);
            _skins[skinIndex].SetActive(true);
            SkinIndexChanged?.Invoke(skinIndex);
        }

        private void Update()
        {
            _animator.SetFloat(n_InputMagnitude, _agent.velocity.magnitude * 0.25f);

            #region Transform - Update
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
            #endregion Transform - Update

            #region Animator - Update
            OnInputHorizontalChanged(_inputHorizontal);
            OnInputVerticalChanged(_inputVertical);
            OnInputMagnitudeChanged(_inputMagnitude);
            OnGroundDistanceChanged(_groundDistance);

            OnIsCrouchingChanged(_isCrouching);
            OnIsGroundedChanged(_isGrounded);
            OnIsSprintingChanged(_isSprinting);
            OnIsStrafingChanged(_isStrafing);
            #endregion Animator - Update
        }

        #region Animator - Actions

        private void OnInputHorizontalChanged(float inputHorizontal)
        {
            if (Mathf.Abs(inputHorizontal - _oldInputHorizontal) < _minAnimatorFloatStep)
            {
                _oldInputHorizontal = inputHorizontal;
                InputHorizontalChanged?.Invoke(inputHorizontal);
            }
        }

        private void OnInputVerticalChanged(float inputVertical)
        {
            if (Mathf.Abs(inputVertical - _oldInputVertical) < _minAnimatorFloatStep)
            {
                _oldInputVertical = inputVertical;
                InputVerticalChanged?.Invoke(inputVertical);
            }
        }

        private void OnInputMagnitudeChanged(float inputMagnitude)
        {
            if (Mathf.Abs(inputMagnitude - _oldInputMagnitude) < _minAnimatorFloatStep)
            {
                _oldInputMagnitude = inputMagnitude;
                InputMagnitudeChanged?.Invoke(inputMagnitude);
            }
        }

        private void OnGroundDistanceChanged(float groundDistance)
        {
            if (Mathf.Abs(groundDistance - _oldGroundDistance) < _minAnimatorFloatStep)
            {
                _oldGroundDistance = groundDistance;
                GroundDistanceChanged?.Invoke(groundDistance);
            }
        }

        private void OnIsCrouchingChanged(bool isCrouching)
        {
            if (isCrouching != _oldIsCrouching)
            {
                _oldIsCrouching = isCrouching;
                IsCrouchingChanged?.Invoke(isCrouching);
            }
        }

        private void OnIsGroundedChanged(bool isGrounded)
        {
            if (isGrounded != _oldIsGrounded)
            {
                _oldIsGrounded = isGrounded;
                IsGroundedChanged?.Invoke(isGrounded);
            }
        }

        private void OnIsSprintingChanged(bool isSprinting)
        {
            if (isSprinting != _oldIsSprinting)
            {
                _oldIsSprinting = isSprinting;
                IsSprintingChanged?.Invoke(isSprinting);
            }
        }

        private void OnIsStrafingChanged(bool isStrafing)
        {
            if (isStrafing != _oldIsStrafing)
            {
                _oldIsStrafing = isStrafing;
                IsStrafingChanged?.Invoke(isStrafing);
            }
        }
        #endregion Animator - Actions
    }
}