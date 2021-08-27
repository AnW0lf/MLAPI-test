using Invector.vCharacterController;
using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class LocalPlayer : MonoBehaviour
    {
        [SerializeField] private Transform _body = null;
        [SerializeField] private HandMotion _hand = null;
        [SerializeField] private vThirdPersonController _tp_controller = null;
        [Header("Minimum difference")]
        [SerializeField] private float _minPositionStep = 0.2f;
        [SerializeField] private float _minRotationStep = 1.5f;
        [SerializeField] private float _minAnimatorFloatStep = 0.1f;

        #region Transform
        private Vector3 _position => _body.position;
        private Vector3 _oldPosition = Vector3.zero;
        public event Action<Vector3> PositionChanged;
        private Quaternion _rotation => _body.rotation;
        private Quaternion _oldRotation = Quaternion.identity;
        public event Action<Quaternion> RotationChanged;
        #endregion Transform

        #region Animator
        private float _oldInputHorizontal = 0f;
        public event Action<float> InputHorizontalChanged;

        private float _oldInputVertical = 0f;
        public event Action<float> InputVerticalChanged;

        private float _oldInputMagnitude = 0f;
        public event Action<float> InputMagnitudeChanged;

        private bool _oldIsGrounded = false;
        public event Action<bool> IsGroundedChanged;

        private bool _oldIsStrafing = false;
        public event Action<bool> IsStrafingChanged;

        private bool _oldIsSprinting = false;
        public event Action<bool> IsSprintingChanged;

        private bool _oldIsCrouching = false;
        public event Action<bool> IsCrouchingChanged;

        private float _oldGroundDistance = 0f;
        public event Action<float> GroundDistanceChanged;
        #endregion Animator

        #region Hand
        private bool _isHandVisible => _hand.HandVisible;
        private bool _oldIsHandVisible = false;
        public event Action<bool> IsHandVisibleChanged;
        #endregion Hand

        public NetworkLocalPlayer NetworkParent { get; set; } = null;

        private void Start()
        {
            SubscribeToTPAnimator();
        }

        private void Update()
        {
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

            #region Hand - Update
            if (_isHandVisible != _oldIsHandVisible)
            {
                _oldIsHandVisible = _isHandVisible;
                IsHandVisibleChanged?.Invoke(_isHandVisible);
            }
            #endregion Hand - Update
        }

        private void OnDestroy()
        {
            UnsubscribeFromTPAnimator();
        }

        #region Animator - Actions
        private bool _subscribedToTPAnimator = false;
        private void SubscribeToTPAnimator()
        {
            if (_tp_controller == null) { return; }
            if (_subscribedToTPAnimator) { return; }

            _tp_controller.InputHorizontalChanged += OnInputHorizontalChanged;
            _tp_controller.InputVerticalChanged += OnInputVerticalChanged;
            _tp_controller.InputMagnitudeChanged += OnInputMagnitudeChanged;
            _tp_controller.GroundDistanceChanged += OnGroundDistanceChanged;

            _tp_controller.IsCrouchingChanged += OnIsCrouchingChanged;
            _tp_controller.IsGroundedChanged += OnIsGroundedChanged;
            _tp_controller.IsSprintingChanged += OnIsSprintingChanged;
            _tp_controller.IsStrafingChanged += OnIsStrafingChanged;

            _subscribedToTPAnimator = true;
        }

        private void UnsubscribeFromTPAnimator()
        {
            if (_tp_controller == null) { return; }
            if (_subscribedToTPAnimator == false) { return; }

            _tp_controller.InputHorizontalChanged -= OnInputHorizontalChanged;
            _tp_controller.InputVerticalChanged -= OnInputVerticalChanged;
            _tp_controller.InputMagnitudeChanged -= OnInputMagnitudeChanged;
            _tp_controller.GroundDistanceChanged -= OnGroundDistanceChanged;

            _tp_controller.IsCrouchingChanged -= OnIsCrouchingChanged;
            _tp_controller.IsGroundedChanged -= OnIsGroundedChanged;
            _tp_controller.IsSprintingChanged -= OnIsSprintingChanged;
            _tp_controller.IsStrafingChanged -= OnIsStrafingChanged;

            _subscribedToTPAnimator = false;
        }

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