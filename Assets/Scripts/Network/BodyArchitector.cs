using System;
using UnityEngine;
using MLAPI.NetworkVariable;

namespace Assets.Scripts.Network
{
    public class BodyArchitector : Architector
    {
        #region Network Variables

        #region Transform
        public readonly NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Vector3.zero);

        public readonly NetworkVariableQuaternion Rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, Quaternion.identity);
        #endregion Transform

        #region Animator

        public readonly NetworkVariableFloat GroundDistance = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);

        public readonly NetworkVariableFloat InputHorizontal = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);

        public readonly NetworkVariableFloat InputMagnitude = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);

        public readonly NetworkVariableFloat InputVertical = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0f);

        public readonly NetworkVariableBool IsCrouching = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        public readonly NetworkVariableBool IsGrounded = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        public readonly NetworkVariableBool IsSprinting = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        public readonly NetworkVariableBool IsStrafing = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        #endregion Animator

        #region Hand
        public readonly NetworkVariableBool IsHandVisible = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);
        #endregion Hand

        #region Skin
        public readonly NetworkVariableInt SkinIndex = new NetworkVariableInt(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, -1);
        #endregion Skin

        #endregion Network Variables

        #region OnLocalChanged Actions

        #region Transform
        private Action<Vector3> _positionChanged = null;
        private Action<Quaternion> _rotationChanged = null;
        #endregion Transform

        #region Animator
        private Action<float> _groundDistanceChanged = null;
        private Action<float> _inputHorizontalChanged = null;
        private Action<float> _inputMagnitudeChanged = null;
        private Action<float> _inputVerticalChanged = null;

        private Action<bool> _isCrouchingChanged = null;
        private Action<bool> _isGroundedChanged = null;
        private Action<bool> _isSprintingChanged = null;
        private Action<bool> _isStrafingChanged = null;
        #endregion Animator

        #region Hand
        private Action<bool> _isHandVisibleChanged = null;
        #endregion Hand

        #region Skin
        private Action<int> _skinIndexChanged = null;
        #endregion Skin

        #endregion OnLocalChanged Actions

        protected override void InitializeActions()
        {
            _positionChanged = (value) => Position.Value = value;
            _rotationChanged = (value) => Rotation.Value = value;

            _groundDistanceChanged = (value) => GroundDistance.Value = value;
            _inputHorizontalChanged = (value) => InputHorizontal.Value = value;
            _inputMagnitudeChanged = (value) => InputMagnitude.Value = value;
            _inputVerticalChanged = (value) => InputVertical.Value = value;

            _isCrouchingChanged = (value) => IsCrouching.Value = value;
            _isGroundedChanged = (value) => IsGrounded.Value = value;
            _isSprintingChanged = (value) => IsSprinting.Value = value;
            _isStrafingChanged = (value) => IsStrafing.Value = value;

            _isHandVisibleChanged = (value) => IsHandVisible.Value = value;

            _skinIndexChanged = (value) => SkinIndex.Value = value;
        }

        protected override void Synchronize()
        {
            LocalBody localBody = Local as LocalBody;

            Position.Value = localBody.Position.Value;
            Rotation.Value = localBody.Rotation.Value;

            GroundDistance.Value = localBody.GroundDistance.Value;
            InputHorizontal.Value = localBody.InputHorizontal.Value;
            InputMagnitude.Value = localBody.InputMagnitude.Value;
            InputVertical.Value = localBody.InputVertical.Value;

            IsCrouching.Value = localBody.IsCrouching.Value;
            IsGrounded.Value = localBody.IsGrounded.Value;
            IsSprinting.Value = localBody.IsSprinting.Value;
            IsStrafing.Value = localBody.IsStrafing.Value;

            IsHandVisible.Value = localBody.IsHandVisible.Value;

            SkinIndex.Value = localBody.SkinIndex.Value;
        }

        protected override void Subscribe()
        {
            LocalBody localBody = Local as LocalBody;

            localBody.Position.ValueChanged += _positionChanged;
            localBody.Rotation.ValueChanged += _rotationChanged;

            localBody.GroundDistance.ValueChanged += _groundDistanceChanged;
            localBody.InputHorizontal.ValueChanged += _inputHorizontalChanged;
            localBody.InputMagnitude.ValueChanged += _inputMagnitudeChanged;
            localBody.InputVertical.ValueChanged += _inputVerticalChanged;

            localBody.IsCrouching.ValueChanged += _isCrouchingChanged;
            localBody.IsGrounded.ValueChanged += _isGroundedChanged;
            localBody.IsSprinting.ValueChanged += _isSprintingChanged;
            localBody.IsStrafing.ValueChanged += _isStrafingChanged;

            localBody.IsHandVisible.ValueChanged += _isHandVisibleChanged;

            localBody.SkinIndex.ValueChanged += _skinIndexChanged;
        }

        protected override void Unsubscribe()
        {
            LocalBody localBody = Local as LocalBody;

            localBody.Position.ValueChanged -= _positionChanged;
            localBody.Rotation.ValueChanged -= _rotationChanged;

            localBody.GroundDistance.ValueChanged -= _groundDistanceChanged;
            localBody.InputHorizontal.ValueChanged -= _inputHorizontalChanged;
            localBody.InputMagnitude.ValueChanged -= _inputMagnitudeChanged;
            localBody.InputVertical.ValueChanged -= _inputVerticalChanged;

            localBody.IsCrouching.ValueChanged -= _isCrouchingChanged;
            localBody.IsGrounded.ValueChanged -= _isGroundedChanged;
            localBody.IsSprinting.ValueChanged -= _isSprintingChanged;
            localBody.IsStrafing.ValueChanged -= _isStrafingChanged;

            localBody.IsHandVisible.ValueChanged -= _isHandVisibleChanged;

            localBody.SkinIndex.ValueChanged -= _skinIndexChanged;
        }
    }
}