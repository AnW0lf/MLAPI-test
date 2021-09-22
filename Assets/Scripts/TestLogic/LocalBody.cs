using System;
using UnityEngine;

namespace Assets.Scripts.TestLogic
{
    public class LocalBody : Local
    {
        [Header("Components")]
        [SerializeField] private Transform _transform = null;
        [SerializeField] private Animator _animator = null;
        [Header("Minimum offset")]
        [SerializeField] private float _positionOffset = 0.15f;
        [SerializeField] private float _rotationOffset = 1.5f;
        [SerializeField] private float _animationOffset = 0.1f;

        #region Local Variables

        #region Transform
        public LocalVector3 Position { get; private set; }
        public LocalQuaternion Rotation { get; private set; }
        #endregion Transform

        #region Animator
        public LocalFloat GroundDistance { get; private set; }
        public LocalFloat InputHorizontal { get; private set; }
        public LocalFloat InputMagnitude { get; private set; }
        public LocalFloat InputVertical { get; private set; }
        public LocalBool IsCrouching { get; private set; }
        public LocalBool IsGrounded { get; private set; }
        public LocalBool IsSprinting { get; private set; }
        public LocalBool IsStrafing { get; private set; }
        #endregion Animator

        #endregion Local Variables


        protected override void InitializeVariables()
        {
            #region Initialize Variables - Transform

            Position = new LocalVector3(_transform.position, _positionOffset);
            Rotation = new LocalQuaternion(_transform.rotation, _rotationOffset);

            #endregion Initialize Variables - Transform

            #region Initialize Variables - Animator

            GroundDistance = new LocalFloat(_animator.GetFloat(AnimatorParameters.GroundDistance), _animationOffset);
            InputHorizontal = new LocalFloat(_animator.GetFloat(AnimatorParameters.InputHorizontal), _animationOffset);
            InputMagnitude = new LocalFloat(_animator.GetFloat(AnimatorParameters.InputMagnitude), _animationOffset);
            InputVertical = new LocalFloat(_animator.GetFloat(AnimatorParameters.InputVertical), _animationOffset);

            IsCrouching = new LocalBool(_animator.GetBool(AnimatorParameters.IsCrouching));
            IsGrounded = new LocalBool(_animator.GetBool(AnimatorParameters.IsGrounded));
            IsSprinting = new LocalBool(_animator.GetBool(AnimatorParameters.IsSprinting));
            IsStrafing = new LocalBool(_animator.GetBool(AnimatorParameters.IsStrafing));

            #endregion Initialize Variables - Animator
        }

        protected override void UpdateVariables()
        {
            #region Update Variables - Transform

            Position.Value = _transform.position;
            Rotation.Value = _transform.rotation;

            #endregion Update Variables - Transform

            #region Update Variables - Animator

            GroundDistance.Value = _animator.GetFloat(AnimatorParameters.GroundDistance);
            InputHorizontal.Value = _animator.GetFloat(AnimatorParameters.InputHorizontal);
            InputMagnitude.Value = _animator.GetFloat(AnimatorParameters.InputMagnitude);
            InputVertical.Value = _animator.GetFloat(AnimatorParameters.InputVertical);

            IsCrouching.Value = _animator.GetBool(AnimatorParameters.IsCrouching);
            IsGrounded.Value = _animator.GetBool(AnimatorParameters.IsGrounded);
            IsSprinting.Value = _animator.GetBool(AnimatorParameters.IsSprinting);
            IsStrafing.Value = _animator.GetBool(AnimatorParameters.IsStrafing);

            #endregion Update Variables - Animator
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
}