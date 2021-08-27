using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [SerializeField] private GameObject _cameraPrefab = null;
        [SerializeField] private HandMotion _hand = null;
        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;

        #endregion

        protected virtual void Start()
        {
            InitilizeController();
            InitializeTpCamera();
            SunscribeToInput();

            Cursor.lockState = CursorLockMode.Locked;
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type

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
            UnsunscribeFromInput();
        }

        private void SunscribeToInput()
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

            InputController.Singleton.OnToMenuStarted += PauseController;
            InputController.Singleton.OnToGameStarted += PauseController;
        }

        private void UnsunscribeFromInput()
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

        private void HandInput(InputAction.CallbackContext obj)
        {
            _hand.SwitchHand();
            cc.freeSpeed.rotateWithCamera = _hand.HandVisible;
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