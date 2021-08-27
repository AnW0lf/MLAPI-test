
using System;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class vThirdPersonAnimator : vThirdPersonMotor
    {
        #region Variables                

        public const float walkSpeed = 0.5f;
        public const float runningSpeed = 1f;
        public const float sprintSpeed = 1.5f;

        #endregion

        #region Actions

        public event Action<float> InputHorizontalChanged;
        public event Action<float> InputVerticalChanged;
        public event Action<float> InputMagnitudeChanged;
        public event Action<bool> IsGroundedChanged;
        public event Action<bool> IsStrafingChanged;
        public event Action<bool> IsSprintingChanged;
        public event Action<bool> IsCrouchingChanged;
        public event Action<float> GroundDistanceChanged;

        #endregion Actions

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            animator.SetBool(vAnimatorParameters.IsStrafing, isStrafing);
            IsStrafingChanged?.Invoke(isStrafing);

            animator.SetBool(vAnimatorParameters.IsSprinting, isSprinting);
            IsSprintingChanged?.Invoke(isSprinting);

            animator.SetBool(vAnimatorParameters.IsCrouching, isCrouching);
            IsCrouchingChanged?.Invoke(isCrouching);

            animator.SetBool(vAnimatorParameters.IsGrounded, isGrounded);
            IsGroundedChanged?.Invoke(isGrounded);

            animator.SetFloat(vAnimatorParameters.GroundDistance, groundDistance);
            GroundDistanceChanged?.Invoke(groundDistance);

            if (isStrafing)
            {
                animator.SetFloat(vAnimatorParameters.InputHorizontal, stopMove ? 0 : horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                InputHorizontalChanged?.Invoke(animator.GetFloat(vAnimatorParameters.InputHorizontal));

                animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                InputVerticalChanged?.Invoke(animator.GetFloat(vAnimatorParameters.InputVertical));
            }
            else
            {
                animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
                InputVerticalChanged?.Invoke(animator.GetFloat(vAnimatorParameters.InputVertical));
            }

            animator.SetFloat(vAnimatorParameters.InputMagnitude, stopMove ? 0f : inputMagnitude, isStrafing ? strafeSpeed.animationSmooth : freeSpeed.animationSmooth, Time.deltaTime);
            InputMagnitudeChanged?.Invoke(animator.GetFloat(vAnimatorParameters.InputMagnitude));
        }

        public virtual void SetAnimatorMoveSpeed(vMovementSpeed speed)
        {
            Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
            verticalSpeed = relativeInput.z;
            horizontalSpeed = relativeInput.x;

            var newInput = new Vector2(verticalSpeed, horizontalSpeed);

            if (speed.walkByDefault)
                inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, isSprinting ? runningSpeed : walkSpeed);
            else
                inputMagnitude = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
        }
    }

    public static partial class vAnimatorParameters
    {
        public static int InputHorizontal = Animator.StringToHash("InputHorizontal");
        public static int InputVertical = Animator.StringToHash("InputVertical");
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static int IsGrounded = Animator.StringToHash("IsGrounded");
        public static int IsStrafing = Animator.StringToHash("IsStrafing");
        public static int IsSprinting = Animator.StringToHash("IsSprinting");
        public static int IsCrouching = Animator.StringToHash("IsCrouching");
        public static int GroundDistance = Animator.StringToHash("GroundDistance");
    }
}