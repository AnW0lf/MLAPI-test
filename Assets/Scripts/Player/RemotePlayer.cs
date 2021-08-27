using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class RemotePlayer : MonoBehaviour
    {
        [SerializeField] private Transform _body = null;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private HandMotion _hand = null;
        [Header("Smoothness")]
        [SerializeField] private float _positionSmooth = 3f;
        [SerializeField] private float _rotationSmooth = 3f;
        [SerializeField] private float _animationSmooth = 0.2f;
        [Header("Maximum difference to interpolate")]
        [SerializeField] private float _maxPositionStep = 1.5f;
        [SerializeField] private float _maxRotationStep = 30f;
        [SerializeField] private float _maxAnimatorFloatStep = 0.5f;

        private NetworkLocalPlayer _networkPlayer = null;
        public NetworkLocalPlayer NetworkParent => _networkPlayer;

        #region Transform
        private Vector3 _targetPosition = Vector3.zero;
        public Vector3 Position
        {
            get => _body.position;
            private set => _body.position = value;
        }

        private Quaternion _targetRotation = Quaternion.identity;
        public Quaternion Rotation
        {
            get => _body.rotation;
            private set => _body.rotation = value;
        }
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

        private float _inputHorizontal
        {
            get => _animator.GetFloat(n_InputHorizontal);
            set
            {
                if (Mathf.Abs(_inputHorizontal - value) > _maxAnimatorFloatStep) _animator.SetFloat(n_InputHorizontal, value);
                else _animator.SetFloat(n_InputHorizontal, value, _animationSmooth, Time.deltaTime);
            }
        }

        private float _inputVertical
        {
            get => _animator.GetFloat(n_InputVertical);
            set
            {
                if (Mathf.Abs(_inputVertical - value) > _maxAnimatorFloatStep) _animator.SetFloat(n_InputVertical, value);
                else _animator.SetFloat(n_InputVertical, value, _animationSmooth, Time.deltaTime);
            }
        }

        private float _inputMagnitude
        {
            get => _animator.GetFloat(n_InputMagnitude);
            set
            {
                if (Mathf.Abs(_inputMagnitude - value) > _maxAnimatorFloatStep) _animator.SetFloat(n_InputMagnitude, value);
                else _animator.SetFloat(n_InputMagnitude, value, _animationSmooth, Time.deltaTime);
            }
        }

        private float _groundDistance
        {
            get => _animator.GetFloat(n_GroundDistance);
            set
            {
                if (Mathf.Abs(_groundDistance - value) > _maxAnimatorFloatStep) _animator.SetFloat(n_GroundDistance, value);
                else _animator.SetFloat(n_GroundDistance, value, _animationSmooth, Time.deltaTime);
            }
        }
        #endregion Animator

        private void Update()
        {
            #region Transform - Update
            if (Vector3.Distance(Position, _targetPosition) > _maxPositionStep)
            {
                Position = _targetPosition;
            }
            else
            {
                Position = Vector3.Lerp(Position, _targetPosition, _positionSmooth * Time.deltaTime);
            }

            if (Quaternion.Angle(Rotation, _targetRotation) > _maxRotationStep)
            {
                Rotation = _targetRotation;
            }
            else
            {
                Rotation = Quaternion.Lerp(Rotation, _targetRotation, _rotationSmooth * Time.deltaTime);
            }
            #endregion Transform - Update
        }

        private void OnDestroy()
        {
            UnsubscribeFromNetworkPlayer();
        }

        private bool _subscribedToNetwork = false;
        public void SubscribeToNetworkPlayer(NetworkLocalPlayer networkPlayer)
        {
            _networkPlayer = networkPlayer;

            if (_networkPlayer == null) { return; }
            if (_subscribedToNetwork) { return; }

            _networkPlayer.PositionChanged += SetTargetPosition;
            _networkPlayer.RotationChanged += SetTargetRotation;

            _networkPlayer.InputHorizontalChanged += SetInputHorizontalChanged;
            _networkPlayer.InputVerticalChanged += SetInputVerticalChanged;
            _networkPlayer.InputMagnitudeChanged += SetInputMagnitudeChanged;
            _networkPlayer.GroundDistanceChanged += SetGroundDistanceChanged;

            _networkPlayer.IsCrouchingChanged += SetIsCrouchingChanged;
            _networkPlayer.IsGroundedChanged += SetIsGroundedChanged;
            _networkPlayer.IsSprintingChanged += SetIsSprintingChanged;
            _networkPlayer.IsStrafingChanged += SetIsStrafingChanged;

            _networkPlayer.IsHandVisibleChanged += SetIsHandVisibleChanged;

            _subscribedToNetwork = true;
        }

        private void UnsubscribeFromNetworkPlayer()
        {
            if (_networkPlayer == null) { return; }
            if (_subscribedToNetwork == false) { return; }

            _networkPlayer.PositionChanged -= SetTargetPosition;
            _networkPlayer.RotationChanged -= SetTargetRotation;

            _networkPlayer.InputHorizontalChanged -= SetInputHorizontalChanged;
            _networkPlayer.InputVerticalChanged -= SetInputVerticalChanged;
            _networkPlayer.InputMagnitudeChanged -= SetInputMagnitudeChanged;
            _networkPlayer.GroundDistanceChanged -= SetGroundDistanceChanged;

            _networkPlayer.IsCrouchingChanged -= SetIsCrouchingChanged;
            _networkPlayer.IsGroundedChanged -= SetIsGroundedChanged;
            _networkPlayer.IsSprintingChanged -= SetIsSprintingChanged;
            _networkPlayer.IsStrafingChanged -= SetIsStrafingChanged;

            _networkPlayer.IsHandVisibleChanged -= SetIsHandVisibleChanged;

            _subscribedToNetwork = false;
        }

        #region Actions
        #region Transform - Actions
        private void SetTargetPosition(Vector3 position)
        {
            _targetPosition = position;
        }

        private void SetTargetRotation(Quaternion rotation)
        {
            _targetRotation = rotation;
        }
        #endregion Transform - Actions

        #region Animator - Actions
        private void SetInputHorizontalChanged(float inputHorizontal)
        {
            _inputHorizontal = inputHorizontal;
        }

        private void SetInputVerticalChanged(float inputVertical)
        {
            _inputVertical = inputVertical;
        }

        private void SetInputMagnitudeChanged(float inputMagnitude)
        {
            _inputMagnitude = inputMagnitude;
        }

        private void SetGroundDistanceChanged(float groundDistance)
        {
            _groundDistance = groundDistance;
        }

        private void SetIsCrouchingChanged(bool isCrouching)
        {
            _animator.SetBool(n_IsCrouching, isCrouching);
        }

        private void SetIsGroundedChanged(bool isGrounded)
        {
            _animator.SetBool(n_IsGrounded, isGrounded);
        }

        private void SetIsSprintingChanged(bool isSprinting)
        {
            _animator.SetBool(n_IsSprinting, isSprinting);
        }

        private void SetIsStrafingChanged(bool isStrafing)
        {
            _animator.SetBool(n_IsStrafing, isStrafing);
        }
        #endregion Animator - Actions

        #region Hand - Actions
        private void SetIsHandVisibleChanged(bool isHandVisible)
        {
            _hand.HandVisible = isHandVisible;
        }
        #endregion Hand - Actions
        #endregion Actions
    }
}