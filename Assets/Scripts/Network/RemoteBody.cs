using UnityEngine;
using MLAPI.NetworkVariable;
using Assets.Scripts.Player;

namespace Assets.Scripts.Network
{
    public class RemoteBody : Remote
    {
        [Header("Components")]
        [SerializeField] private Transform _transform = null;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private HandMotion _handMotion = null;
        [SerializeField] private Skinner _skinner = null;
        [Header("Maximum offset")]
        [SerializeField] private float _positionOffset = 1f;
        [SerializeField] private float _rotationOffset = 30f;
        [SerializeField] private float _animationOffset = 0.5f;
        [Space(25)]
        [SerializeField] private float _smoothness = 3f;

        #region Remote Variables

        #region Transform
        private RemoteVector3 _position = null;
        private RemoteQuaternion _rotation = null;
        #endregion Transform

        #region Animator
        private RemoteFloat _groundDistance = null;
        private RemoteFloat _inputHorizontal = null;
        private RemoteFloat _inputMagnitude = null;
        private RemoteFloat _inputVertical = null;

        private RemoteBool _isCrouching = null;
        private RemoteBool _isGrounded = null;
        private RemoteBool _isSprinting = null;
        private RemoteBool _isStrafing = null;
        #endregion Animator

        #region Hand
        private RemoteBool _isHandVisible = null;
        #endregion Hand

        #region Skin
        private RemoteInt _skinIndex = null;
        #endregion Skin

        #endregion Remote Variables

        #region Remote Actions

        #region Transform
        private NetworkVariableVector3.OnValueChangedDelegate _positionChanged;
        private NetworkVariableQuaternion.OnValueChangedDelegate _rotationChanged;
        #endregion Transform

        #region Animator
        private NetworkVariableFloat.OnValueChangedDelegate _groundDistanceChanged;
        private NetworkVariableFloat.OnValueChangedDelegate _inputHorizontalChanged;
        private NetworkVariableFloat.OnValueChangedDelegate _inputMagnitudeChanged;
        private NetworkVariableFloat.OnValueChangedDelegate _inputVerticalChanged;

        private NetworkVariableBool.OnValueChangedDelegate _isCrouchingChanged;
        private NetworkVariableBool.OnValueChangedDelegate _isGroundedChanged;
        private NetworkVariableBool.OnValueChangedDelegate _isSprintingChanged;
        private NetworkVariableBool.OnValueChangedDelegate _isStrafingChanged;
        #endregion Animator

        #region Hand
        private NetworkVariableBool.OnValueChangedDelegate _isHandVisibleChanged;
        #endregion Hand

        #region Hand
        private NetworkVariableInt.OnValueChangedDelegate _skinIndexChanged;
        #endregion Hand

        #endregion Remote Actions


        protected override void InitializeVariables()
        {
            BodyArchitector bodyArchitector = Architector as BodyArchitector;

            #region Initialize Variables - Transform

            _position = new RemoteVector3(bodyArchitector.Position.Value, _positionOffset)
            {
                IsEquals = (target) => Vector3.Distance(target, _transform.position) == 0f,
                IsOverOffset = (target, offset) => Vector3.Distance(target, _transform.position) > offset
            };
            _position.SetValue += (value) => _transform.position = value;
            _position.UpdateValue += (value) => _transform.position = Vector3.Lerp(_transform.position, value, _smoothness * Time.deltaTime);

            _rotation = new RemoteQuaternion(bodyArchitector.Rotation.Value, _rotationOffset)
            {
                IsEquals = (target) => Quaternion.Angle(target, _transform.rotation) == 0f,
                IsOverOffset = (target, offset) => Quaternion.Angle(target, _transform.rotation) > offset
            };
            _rotation.SetValue += (value) => _transform.rotation = value;
            _rotation.UpdateValue += (value) => _transform.rotation = Quaternion.Lerp(_transform.rotation, value, _smoothness * Time.deltaTime);

            #endregion Initialize Variables - Transform

            #region Initialize Variables - Animator

            _groundDistance = new RemoteFloat(bodyArchitector.GroundDistance.Value, _animationOffset)
            {
                IsEquals = (target) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.GroundDistance)) == 0f,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.GroundDistance)) > offset
            };
            _groundDistance.SetValue += (value) => _animator.SetFloat(AnimatorParameters.GroundDistance, value);
            _groundDistance.UpdateValue += (value) => _animator.SetFloat(AnimatorParameters.GroundDistance
                , Mathf.Lerp(_animator.GetFloat(AnimatorParameters.GroundDistance)
                , value, _smoothness * Time.deltaTime));

            _inputHorizontal = new RemoteFloat(bodyArchitector.InputHorizontal.Value, _animationOffset)
            {
                IsEquals = (target) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputHorizontal)) == 0f,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputHorizontal)) > offset
            };
            _inputHorizontal.SetValue += (value) => _animator.SetFloat(AnimatorParameters.InputHorizontal, value);
            _inputHorizontal.UpdateValue += (value) => _animator.SetFloat(AnimatorParameters.InputHorizontal
                , Mathf.Lerp(_animator.GetFloat(AnimatorParameters.InputHorizontal)
                , value, _smoothness * Time.deltaTime));

            _inputMagnitude = new RemoteFloat(bodyArchitector.InputMagnitude.Value, _animationOffset)
            {
                IsEquals = (target) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputMagnitude)) == 0f,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputMagnitude)) > offset
            };
            _inputMagnitude.SetValue += (value) => _animator.SetFloat(AnimatorParameters.InputMagnitude, value);
            _inputMagnitude.UpdateValue += (value) => _animator.SetFloat(AnimatorParameters.InputMagnitude
                , Mathf.Lerp(_animator.GetFloat(AnimatorParameters.InputMagnitude)
                , value, _smoothness * Time.deltaTime));

            _inputVertical = new RemoteFloat(bodyArchitector.InputVertical.Value, _animationOffset)
            {
                IsEquals = (target) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputVertical)) == 0f,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputVertical)) > offset
            };
            _inputVertical.SetValue += (value) => _animator.SetFloat(AnimatorParameters.InputVertical, value);
            _inputVertical.UpdateValue += (value) => _animator.SetFloat(AnimatorParameters.InputVertical
                , Mathf.Lerp(_animator.GetFloat(AnimatorParameters.InputVertical)
                , value, _smoothness * Time.deltaTime));

            _isCrouching = new RemoteBool(bodyArchitector.IsCrouching.Value)
            {
                IsEquals = (target) => target == _animator.GetBool(AnimatorParameters.IsCrouching)
            };
            _isCrouching.SetValue += (value) => _animator.SetBool(AnimatorParameters.IsCrouching, value);

            _isGrounded = new RemoteBool(bodyArchitector.IsGrounded.Value)
            {
                IsEquals = (target) => target == _animator.GetBool(AnimatorParameters.IsGrounded)
            };
            _isGrounded.SetValue += (value) => _animator.SetBool(AnimatorParameters.IsGrounded, value);

            _isSprinting = new RemoteBool(bodyArchitector.IsSprinting.Value)
            {
                IsEquals = (target) => target == _animator.GetBool(AnimatorParameters.IsSprinting)
            };
            _isSprinting.SetValue += (value) => _animator.SetBool(AnimatorParameters.IsSprinting, value);

            _isStrafing = new RemoteBool(bodyArchitector.IsStrafing.Value)
            {
                IsEquals = (target) => target == _animator.GetBool(AnimatorParameters.IsStrafing)
            };
            _isStrafing.SetValue += (value) => _animator.SetBool(AnimatorParameters.IsStrafing, value);

            #endregion Initialize Variables - Animator

            #region Initialize Variables - Hand

            _isHandVisible = new RemoteBool(bodyArchitector.IsHandVisible.Value)
            {
                IsEquals = (target) => target == _handMotion.IsHandVisible
            };
            _isHandVisible.SetValue += (value) => _handMotion.IsHandVisible = value;

            #endregion Initialize Variables - Hand

            #region Initialize Variables - Skin

            _skinIndex = new RemoteInt(bodyArchitector.SkinIndex.Value, 0)
            {
                IsEquals = (target) => target == _skinner.SkinIndex,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _skinner.SkinIndex) > offset
            };
            _skinIndex.SetValue += (value) => _skinner.SkinIndex = value;
            _skinIndex.UpdateValue += (value) => _skinner.SkinIndex = value;

            #endregion Initialize Variables - Skin
        }

        protected override void InitializeArchitectorActions()
        {
            #region Initialize Actions - Transform

            _positionChanged = (oldValue, newValue) => _position.Target = newValue;
            _rotationChanged = (oldValue, newValue) => _rotation.Target = newValue;

            #endregion Initialize Actions - Transform

            #region Initialize Actions - Animator

            _groundDistanceChanged = (oldValue, newValue) => _groundDistance.Target = newValue;
            _inputHorizontalChanged = (oldValue, newValue) => _inputHorizontal.Target = newValue;
            _inputMagnitudeChanged = (oldValue, newValue) => _inputMagnitude.Target = newValue;
            _inputVerticalChanged = (oldValue, newValue) => _inputVertical.Target = newValue;

            _isCrouchingChanged = (oldValue, newValue) => _isCrouching.Target = newValue;
            _isGroundedChanged = (oldValue, newValue) => _isGrounded.Target = newValue;
            _isSprintingChanged = (oldValue, newValue) => _isSprinting.Target = newValue;
            _isStrafingChanged = (oldValue, newValue) => _isStrafing.Target = newValue;

            #endregion Initialize Actions - Animator

            #region Initialize Actions - Hand

            _isHandVisibleChanged = (oldValue, newValue) => _handMotion.IsHandVisible = newValue;

            #endregion Initialize Actions - Hand

            #region Initialize Actions - Skin

            _skinIndexChanged = (oldValue, newValue) => _skinner.SkinIndex = newValue;

            #endregion Initialize Actions - Skin
        }

        protected override void Subscribe()
        {
            BodyArchitector bodyArchitector = Architector as BodyArchitector;

            #region Subscribe - Transform

            bodyArchitector.Position.OnValueChanged += _positionChanged;
            bodyArchitector.Rotation.OnValueChanged += _rotationChanged;

            #endregion Subscribe - Transform

            #region Subscribe - Animator

            bodyArchitector.GroundDistance.OnValueChanged += _groundDistanceChanged;
            bodyArchitector.InputHorizontal.OnValueChanged += _inputHorizontalChanged;
            bodyArchitector.InputMagnitude.OnValueChanged += _inputMagnitudeChanged;
            bodyArchitector.InputVertical.OnValueChanged += _inputVerticalChanged;

            bodyArchitector.IsCrouching.OnValueChanged += _isCrouchingChanged;
            bodyArchitector.IsGrounded.OnValueChanged += _isGroundedChanged;
            bodyArchitector.IsSprinting.OnValueChanged += _isSprintingChanged;
            bodyArchitector.IsStrafing.OnValueChanged += _isStrafingChanged;

            #endregion Subscribe - Animator

            #region Subscribe - Hand

            bodyArchitector.IsHandVisible.OnValueChanged += _isHandVisibleChanged;

            #endregion Subscribe - Hand

            #region Subscribe - Skin

            bodyArchitector.SkinIndex.OnValueChanged += _skinIndexChanged;

            #endregion Subscribe - Skin
        }

        protected override void Unsubscribe()
        {
            BodyArchitector bodyArchitector = Architector as BodyArchitector;

            #region Unsubscribe - Transform

            bodyArchitector.Position.OnValueChanged -= _positionChanged;
            bodyArchitector.Rotation.OnValueChanged -= _rotationChanged;

            #endregion Unsubscribe - Transform

            #region Unsubscribe - Animator

            bodyArchitector.GroundDistance.OnValueChanged -= _groundDistanceChanged;
            bodyArchitector.InputHorizontal.OnValueChanged -= _inputHorizontalChanged;
            bodyArchitector.InputMagnitude.OnValueChanged -= _inputMagnitudeChanged;
            bodyArchitector.InputVertical.OnValueChanged -= _inputVerticalChanged;

            bodyArchitector.IsCrouching.OnValueChanged -= _isCrouchingChanged;
            bodyArchitector.IsGrounded.OnValueChanged -= _isGroundedChanged;
            bodyArchitector.IsSprinting.OnValueChanged -= _isSprintingChanged;
            bodyArchitector.IsStrafing.OnValueChanged -= _isStrafingChanged;

            #endregion Unsubscribe - Animator

            #region Subscribe - Hand

            bodyArchitector.IsHandVisible.OnValueChanged -= _isHandVisibleChanged;

            #endregion Subscribe - Hand

            #region Subscribe - Skin

            bodyArchitector.SkinIndex.OnValueChanged -= _skinIndexChanged;

            #endregion Subscribe - Skin
        }

        protected override void UpdateVariables()
        {
            #region Update Variables - Transform

            _position.Update();
            _rotation.Update();

            #endregion Update Variables - Transform

            #region Update Variables - Animator

            _groundDistance.Update();
            _inputHorizontal.Update();
            _inputMagnitude.Update();
            _inputVertical.Update();

            _isCrouching.Update();
            _isGrounded.Update();
            _isSprinting.Update();
            _isStrafing.Update();

            #endregion Update Variables - Animator

            #region Update Variables - Hand

            _isHandVisible.Update();

            #endregion Update Variables - Hand
        }
    }
}