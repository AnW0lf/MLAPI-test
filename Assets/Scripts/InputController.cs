using Assets.Scripts.Player;
using MLAPI;
using UnityEngine;

namespace Player
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _movement = null;
        [SerializeField] private PlayerItemInHand _hand = null;

        private InputManager _inputs = null;
        private InputControllerMap _map = InputControllerMap.MENU;

        public InputControllerMap Map
        {
            get => _map;
            set
            {
                _map = value;

                if (_inputs == null)
                {
                    _inputs = new InputManager();
                }

                switch (_map)
                {
                    case InputControllerMap.GAME:
                        {
                            // Deactive Menu
                            _inputs.Menu.ToGame.performed -= ToGame;
                            _inputs.Menu.Disable();

                            // Active Game
                            _inputs.Game.Move.started += Move;
                            _inputs.Game.Move.performed += Move;
                            _inputs.Game.Move.canceled += Move;

                            _inputs.Game.Rotate.performed += Rotate;

                            _inputs.Game.Jump.performed += Jump;

                            _inputs.Game.FastRunMode.started += SwitchRunMode;
                            _inputs.Game.FastRunMode.canceled += SwitchRunMode;

                            _inputs.Game.RunMode.performed += SwitchRunMode;

                            _inputs.Game.Squat.performed += SwitchSquatMode;

                            _inputs.Game.Weapon.performed += SwitchHand;

                            _inputs.Game.Use.performed += Use;

                            _inputs.Game.ToMenu.performed += ToMenu;

                            _inputs.Game.Enable();
                        }
                        break;
                    case InputControllerMap.MENU:
                        {
                            // Deactive Game
                            _inputs.Game.Move.started -= Move;
                            _inputs.Game.Move.performed -= Move;
                            _inputs.Game.Move.canceled -= Move;

                            _inputs.Game.Rotate.performed -= Rotate;

                            _inputs.Game.Jump.performed -= Jump;

                            _inputs.Game.FastRunMode.started -= SwitchRunMode;
                            _inputs.Game.FastRunMode.canceled -= SwitchRunMode;

                            _inputs.Game.RunMode.performed -= SwitchRunMode;

                            _inputs.Game.Squat.performed -= SwitchSquatMode;

                            _inputs.Game.Weapon.performed -= SwitchHand;

                            _inputs.Game.Use.performed -= Use;

                            _inputs.Game.ToMenu.performed -= ToMenu;

                            _inputs.Game.Disable();

                            // Active Menu
                            _inputs.Menu.ToGame.performed += ToGame;
                            _inputs.Menu.Enable();
                        }
                        break;
                }
            }
        }

        private void Start()
        {
            Map = _map;
        }

        #region Hand
        private void SwitchHand(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _hand.SwitchHand(NetworkManager.Singleton.LocalClientId);
        }

        private void Use(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _hand.Use();
        }
        #endregion Hand

        #region Movement
        private void Move(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _movement.MoveDirection = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
        }

        private void Rotate(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _movement.Rotate(context.ReadValue<Vector2>());
        }

        private void Jump(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _movement.Jump();
        }

        private void SwitchRunMode(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _movement.RunMode = !_movement.RunMode;
        }

        private void SwitchSquatMode(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _movement.SquatMode = !_movement.SquatMode;
        }
        #endregion Movement

        #region Menu
        private void ToGame(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            
        }

        private void ToMenu(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {

        }
        #endregion Menu
    }
    public enum InputControllerMap { GAME, MENU }
}
