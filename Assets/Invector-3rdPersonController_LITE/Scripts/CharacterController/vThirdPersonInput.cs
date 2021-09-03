using Assets.Scripts.Player;
using Assets.Scripts.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [SerializeField] private GameObject _cameraPrefab = null;
        [SerializeField] private HandMotion _hand = null;
        [SerializeField] private TPCWeapon _weapon = null;
        [SerializeField] [Range(-1f, 1f)] private float _horizontalWeaponOffset = 0f;
        [SerializeField] [Range(-1f, 1f)] private float _verticalWeaponOffset = 0f;
        [SerializeField] [Range(0f, 10f)] private float _normalCameraDistance = 2.5f;
        [SerializeField] [Range(0f, 10f)] private float _aimCameraDistance = 0.5f;
        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;

        #endregion

        protected virtual void Start()
        {
            InitilizeController();
            InitializeTpCamera();
            SubscribeToInput();

            Cursor.lockState = CursorLockMode.Locked;
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            if (cc.freeSpeed.rotateWithCamera)
            {
                if (cameraMain)
                {
                    cc.ControlRotationType(cameraMain.transform.forward);       // handle the controller rotation type
                }
            }
            else
            {
                cc.ControlRotationType();       // handle the controller rotation type
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }
        }

        protected virtual void Update()
        {
            cc.UpdateAnimator();            // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        private void OnDestroy()
        {
            UnsubscribeFromInput();
        }

        private void SubscribeToInput()
        {
            if (InputController.Singleton == null) { return; }

            InputController.Singleton.OnMoveStarted += MoveInput;
            InputController.Singleton.OnMovePerformed += MoveInput;
            InputController.Singleton.OnMoveCancelled += MoveInput;

            InputController.Singleton.OnRotateStarted += CameraInput;
            InputController.Singleton.OnRotatePerformed += CameraInput;
            InputController.Singleton.OnRotateCancelled += CameraInput;

            InputController.Singleton.OnSprintStarted += SprintInput;
            InputController.Singleton.OnSprintCancelled += SprintInput;

            InputController.Singleton.OnJumpStarted += JumpInput;

            InputController.Singleton.OnCrouchStarted += CrouchInput;

            InputController.Singleton.OnHandStarted += HandInput;

            InputController.Singleton.OnAimStarted += AimInput;
            InputController.Singleton.OnAimCancelled += AimInput;

            InputController.Singleton.OnUseStarted += UseInput;

            InputController.Singleton.OnToMenuStarted += PauseController;
            InputController.Singleton.OnToGameStarted += PauseController;
        }

        private void UnsubscribeFromInput()
        {
            if (InputController.Singleton == null) { return; }

            InputController.Singleton.OnMoveStarted -= MoveInput;
            InputController.Singleton.OnMovePerformed -= MoveInput;
            InputController.Singleton.OnMoveCancelled -= MoveInput;

            InputController.Singleton.OnRotateStarted -= CameraInput;
            InputController.Singleton.OnRotatePerformed -= CameraInput;
            InputController.Singleton.OnRotateCancelled -= CameraInput;

            InputController.Singleton.OnSprintStarted -= SprintInput;
            InputController.Singleton.OnSprintCancelled -= SprintInput;

            InputController.Singleton.OnJumpStarted -= JumpInput;

            InputController.Singleton.OnCrouchStarted -= CrouchInput;

            InputController.Singleton.OnHandStarted -= HandInput;

            InputController.Singleton.OnAimStarted -= AimInput;
            InputController.Singleton.OnAimCancelled -= AimInput;

            InputController.Singleton.OnUseStarted -= UseInput;

            InputController.Singleton.OnToMenuStarted -= PauseController;
            InputController.Singleton.OnToGameStarted -= PauseController;
        }

        private void PauseController(bool pause)
        {
            if (InputController.Singleton.Map == InputControllerMap.GAME)
            {
                InputController.Singleton.Map = InputControllerMap.MENU;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (InputController.Singleton.Map == InputControllerMap.MENU)
            {
                InputController.Singleton.Map = InputControllerMap.GAME;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            cameraMain = Instantiate(_cameraPrefab).GetComponent<Camera>();

            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }

                _hand.tpCamera = tpCamera;
            }
        }

        public virtual void MoveInput(Vector2 direction)
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            cc.input.x = direction.x;
            cc.input.z = direction.y;
        }

        protected virtual void CameraInput(Vector2 delta)
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (tpCamera == null)
                return;

            var Y = delta.y;
            var X = delta.x;

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput(bool isStrafe)
        {
            if (isStrafe)
                cc.Strafe();
        }

        protected virtual void SprintInput(bool isSprint)
        {
            cc.Sprint(isSprint);
        }

        protected virtual void CrouchInput(bool isCrouch)
        {
            cc.Crouch(!cc.isCrouching);
        }

        private void HandInput(InputAction.CallbackContext context)
        {
            _hand.SwitchHand();
            AimInput(false);
        }

        private void AimInput(bool press)
        {
            bool active = press && _hand.HandVisible;

            cc.freeSpeed.rotateWithCamera = active;
            tpCamera.rightOffset = active ? _horizontalWeaponOffset : 0f;
            tpCamera.upOffset = active ? _verticalWeaponOffset : 0f;
            tpCamera.defaultDistance = active ? _aimCameraDistance : _normalCameraDistance;
            _hand.IsAim = active;
        }

        private void UseInput(bool use)
        {
            if (_hand.HandVisible == false) { return; }
            if (_hand.IsAim == false) { return; }

            _weapon.Shoot();
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump
        /// </summary>
        protected virtual void JumpInput(bool isJump)
        {
            if (isJump && JumpConditions())
                cc.Jump();
        }

        #endregion       
    }
}