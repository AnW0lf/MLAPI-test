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

        BodyArchitector bodyArchitector = null;

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

        protected override void InitializeVariables()
        {
            #region Initialize Variables - Transform

            _position = new RemoteVector3(bodyArchitector.Position.Value, _positionOffset)
            {
                IsEquals = (target) => Vector3.Distance(target, _transform.position) == 0f,
                IsOverOffset = (target, offset) => Vector3.Distance(target, _transform.position) > offset
            };
            _position.SetValue += (value) => _transform.position = value;
            _position.UpdateValue += (value) => _transform.position = Vector3.Lerp(_transform.position, value, _smoothness * Time.fixedDeltaTime);
            _transform.position = _position.Target;

            _rotation = new RemoteQuaternion(bodyArchitector.Rotation.Value, _rotationOffset)
            {
                IsEquals = (target) => Quaternion.Angle(target, _transform.rotation) == 0f,
                IsOverOffset = (target, offset) => Quaternion.Angle(target, _transform.rotation) > offset
            };
            _rotation.SetValue += (value) => _transform.rotation = value;
            _rotation.UpdateValue += (value) => _transform.rotation = Quaternion.Lerp(_transform.rotation, value, _smoothness * Time.fixedDeltaTime);
            _transform.rotation = _rotation.Target;

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
                , value, _smoothness * Time.fixedDeltaTime));
            _animator.SetFloat(AnimatorParameters.GroundDistance, _groundDistance.Target);

            _inputHorizontal = new RemoteFloat(bodyArchitector.InputHorizontal.Value, _animationOffset)
            {
                IsEquals = (target) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputHorizontal)) == 0f,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputHorizontal)) > offset
            };
            _inputHorizontal.SetValue += (value) => _animator.SetFloat(AnimatorParameters.InputHorizontal, value);
            _inputHorizontal.UpdateValue += (value) => _animator.SetFloat(AnimatorParameters.InputHorizontal
                , Mathf.Lerp(_animator.GetFloat(AnimatorParameters.InputHorizontal)
                , value, _smoothness * Time.fixedDeltaTime));
            _animator.SetFloat(AnimatorParameters.InputHorizontal, _inputHorizontal.Target);

            _inputMagnitude = new RemoteFloat(bodyArchitector.InputMagnitude.Value, _animationOffset)
            {
                IsEquals = (target) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputMagnitude)) == 0f,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputMagnitude)) > offset
            };
            _inputMagnitude.SetValue += (value) => _animator.SetFloat(AnimatorParameters.InputMagnitude, value);
            _inputMagnitude.UpdateValue += (value) => _animator.SetFloat(AnimatorParameters.InputMagnitude
                , Mathf.Lerp(_animator.GetFloat(AnimatorParameters.InputMagnitude)
                , value, _smoothness * Time.fixedDeltaTime));
            _animator.SetFloat(AnimatorParameters.InputMagnitude, _inputHorizontal.Target);

            _inputVertical = new RemoteFloat(bodyArchitector.InputVertical.Value, _animationOffset)
            {
                IsEquals = (target) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputVertical)) == 0f,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _animator.GetFloat(AnimatorParameters.InputVertical)) > offset
            };
            _inputVertical.SetValue += (value) => _animator.SetFloat(AnimatorParameters.InputVertical, value);
            _inputVertical.UpdateValue += (value) => _animator.SetFloat(AnimatorParameters.InputVertical
                , Mathf.Lerp(_animator.GetFloat(AnimatorParameters.InputVertical)
                , value, _smoothness * Time.fixedDeltaTime));
            _animator.SetFloat(AnimatorParameters.InputVertical, _inputVertical.Target);

            _isCrouching = new RemoteBool(bodyArchitector.IsCrouching.Value)
            {
                IsEquals = (target) => target == _animator.GetBool(AnimatorParameters.IsCrouching)
            };
            _isCrouching.SetValue += (value) => _animator.SetBool(AnimatorParameters.IsCrouching, value);
            _animator.SetBool(AnimatorParameters.IsCrouching, _isCrouching.Target);

            _isGrounded = new RemoteBool(bodyArchitector.IsGrounded.Value)
            {
                IsEquals = (target) => target == _animator.GetBool(AnimatorParameters.IsGrounded)
            };
            _isGrounded.SetValue += (value) => _animator.SetBool(AnimatorParameters.IsGrounded, value);
            _animator.SetBool(AnimatorParameters.IsGrounded, _isGrounded.Target);

            _isSprinting = new RemoteBool(bodyArchitector.IsSprinting.Value)
            {
                IsEquals = (target) => target == _animator.GetBool(AnimatorParameters.IsSprinting)
            };
            _isSprinting.SetValue += (value) => _animator.SetBool(AnimatorParameters.IsSprinting, value);
            _animator.SetBool(AnimatorParameters.IsSprinting, _isSprinting.Target);

            _isStrafing = new RemoteBool(bodyArchitector.IsStrafing.Value)
            {
                IsEquals = (target) => target == _animator.GetBool(AnimatorParameters.IsStrafing)
            };
            _isStrafing.SetValue += (value) => _animator.SetBool(AnimatorParameters.IsStrafing, value);
            _animator.SetBool(AnimatorParameters.IsStrafing, _isStrafing.Target);

            #endregion Initialize Variables - Animator

            #region Initialize Variables - Hand

            _isHandVisible = new RemoteBool(bodyArchitector.IsHandVisible.Value)
            {
                IsEquals = (target) => target == _handMotion.IsHandVisible
            };
            _isHandVisible.SetValue += (value) => _handMotion.IsHandVisible = value;
            _handMotion.IsHandVisible = _isHandVisible.Target;

            #endregion Initialize Variables - Hand

            #region Initialize Variables - Skin

            _skinIndex = new RemoteInt(bodyArchitector.SkinIndex.Value, 0)
            {
                IsEquals = (target) => target == _skinner.SkinIndex,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _skinner.SkinIndex) > offset
            };
            _skinIndex.SetValue += (value) => _skinner.SkinIndex = value;
            _skinIndex.UpdateValue += (value) => _skinner.SkinIndex = value;
            _skinner.SkinIndex = _skinIndex.Target;

            #endregion Initialize Variables - Skin
        }

        protected override void InitializeArchitector()
        {
            if (Architector == null) bodyArchitector = null;
            else bodyArchitector = Architector as BodyArchitector;
        }

        protected override void UpdateVariables()
        {
            #region Update Variables - Transform

            _position.Target = bodyArchitector.Position.Value;
            _rotation.Target = bodyArchitector.Rotation.Value;

            #endregion Update Variables - Transform

            #region Update Variables - Animator

            _groundDistance.Target = bodyArchitector.GroundDistance.Value;
            _inputHorizontal.Target = bodyArchitector.InputHorizontal.Value;
            _inputMagnitude.Target = bodyArchitector.InputMagnitude.Value;
            _inputVertical.Target = bodyArchitector.InputVertical.Value;

            _isCrouching.Target = bodyArchitector.IsCrouching.Value;
            _isGrounded.Target = bodyArchitector.IsGrounded.Value;
            _isSprinting.Target = bodyArchitector.IsSprinting.Value;
            _isStrafing.Target = bodyArchitector.IsStrafing.Value;

            #endregion Update Variables - Animator

            #region Update Variables - Hand

            _isHandVisible.Target = bodyArchitector.IsHandVisible.Value;

            #endregion Update Variables - Hand

            #region Update Variables - Skin

            _skinIndex.Target = bodyArchitector.SkinIndex.Value;

            #endregion Update Variables - Skin
        }
    }
}