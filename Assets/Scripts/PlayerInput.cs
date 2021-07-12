using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(LocalPlayer))]
    public class PlayerInput : MonoBehaviour
    {
        private InputManager _inputs = null;
        private LocalPlayer _localPlayer = null;

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _localPlayer = GetComponent<LocalPlayer>();
            _inputs = new InputManager();
            Subscribe();
            _inputs.Game.Enable();
        }

        private void OnDisable()
        {
            Unsubscribe();
            _inputs.Game.Disable();
            _inputs = null;
        }

        private void Subscribe()
        {
            _inputs.Game.Move.performed += Move;
            _inputs.Game.Move.canceled += Move;
            _inputs.Game.Rotate.performed += Rotate;
            _inputs.Game.Rotate.canceled += Rotate;
        }

        private void Unsubscribe()
        {
            _inputs.Game.Move.performed -= Move;
            _inputs.Game.Move.canceled -= Move;
            _inputs.Game.Rotate.performed -= Rotate;
            _inputs.Game.Rotate.canceled -= Rotate;
        }

        private void Move(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            print(direction);
            _localPlayer.Direction = direction;
        }

        private void Rotate(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Vector2 delta = context.ReadValue<Vector2>();
            _localPlayer.MouseDelta = delta;
        }
    }
}
