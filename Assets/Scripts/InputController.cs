using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Singleton = null;
    private InputManager _inputs = null;

    private void Awake()
    {
        if (Singleton == null) Singleton = this;
        else if (Singleton != this) Destroy(gameObject);

        _inputs = new InputManager();
        Init();
    }

    private void Start()
    {
        Map = InputControllerMap.GAME;
    }

    private void LateUpdate()
    {
        switch (Map)
        {
            case InputControllerMap.DISABLED:
                break;
            case InputControllerMap.MENU:
                {

                }
                break;
            case InputControllerMap.GAME:
                {
                    // Game - Sprint
                    if (IsSprintDown) IsSprintDown = false;
                    if (IsSprintUp) IsSprintUp = false;
                    // Game - Jump
                    if (IsJumpDown) IsJumpDown = false;
                    if (IsJumpUp) IsJumpUp = false;
                    // Game - Crouch
                    if (IsCrouchDown) IsCrouchDown = false;
                    if (IsCrouchUp) IsCrouchUp = false;
                    // Game - Hand
                    if (IsHandDown) IsHandDown = false;
                    if (IsHandUp) IsHandUp = false;
                    // Game - Use
                    if (IsUseDown) IsUseDown = false;
                    if (IsUseUp) IsUseUp = false;
                    // Game - ToMenu
                    if (IsToMenuDown) IsToMenuDown = false;
                    if (IsToMenuUp) IsToMenuUp = false;

                    // Menu - ToGame
                    if (IsToGameDown) IsToGameDown = false;
                    if (IsToGameUp) IsToGameUp = false;
                }
                break;
        }
    }

    private void OnDestroy()
    {
        Map = InputControllerMap.DISABLED;
    }

    private InputControllerMap _map = InputControllerMap.DISABLED;
    public InputControllerMap Map
    {
        get => _map;
        set
        {
            _map = value;
            switch (_map)
            {
                case InputControllerMap.DISABLED:
                    _inputs.Menu.Disable();
                    _inputs.Game.Disable();
                    break;
                case InputControllerMap.MENU:
                    _inputs.Menu.Enable();
                    _inputs.Game.Disable();
                    break;
                case InputControllerMap.GAME:
                    _inputs.Menu.Disable();
                    _inputs.Game.Enable();
                    break;
            }
            ResetMapData();
        }
    }

    private void Init()
    {
        #region Init - Game - Move
        _inputs.Game.Move.started += MoveStarted;
        _inputs.Game.Move.performed += MovePerformed;
        _inputs.Game.Move.canceled += MoveCancelled;
        #endregion Init - Game - Move

        #region Init - Game - Rotate
        _inputs.Game.Rotate.started += RotateStarted;
        _inputs.Game.Rotate.performed += RotatePerformed;
        _inputs.Game.Rotate.canceled += RotateCancelled;
        #endregion Init - Game - Rotate

        #region Init - Game - Sprint
        _inputs.Game.Sprint.started += SprintStarted;
        _inputs.Game.Sprint.performed += SprintPerformed;
        _inputs.Game.Sprint.canceled += SprintCancelled;
        #endregion Init - Game - Sprint

        #region Init - Game - Jump
        _inputs.Game.Jump.started += JumpStarted;
        _inputs.Game.Jump.performed += JumpPerformed;
        _inputs.Game.Jump.canceled += JumpCancelled;
        #endregion Init - Game - Jump

        #region Init - Game - Crouch
        _inputs.Game.Crouch.started += CrouchStarted;
        _inputs.Game.Crouch.performed += CrouchPerformed;
        _inputs.Game.Crouch.canceled += CrouchCancelled;
        #endregion Init - Game - Crouch

        #region Init - Game - Hand
        _inputs.Game.Hand.started += HandStarted;
        _inputs.Game.Hand.performed += HandPerformed;
        _inputs.Game.Hand.canceled += HandCancelled;
        #endregion Init - Game - Hand

        #region Init - Game - Use
        _inputs.Game.Use.started += UseStarted;
        _inputs.Game.Use.performed += UsePerformed;
        _inputs.Game.Use.canceled += UseCancelled;
        #endregion Init - Game - Use

        #region Init - Game - ToMenu
        _inputs.Game.ToMenu.started += ToMenuStarted;
        _inputs.Game.ToMenu.performed += ToMenuPerformed;
        _inputs.Game.ToMenu.canceled += ToMenuCancelled;
        #endregion Init - Game - ToMenu

        #region Init - Menu - ToGame
        _inputs.Menu.ToGame.started += ToGameStarted;
        _inputs.Menu.ToGame.performed += ToGamePerformed;
        _inputs.Menu.ToGame.canceled += ToGameCancelled;
        #endregion Init - Menu - ToGame

        ResetMapData();
    }

    private void ResetMapData()
    {
        #region Reset - Game
        Direction = Vector2.zero;
        MouseDelta = Vector2.zero;
        IsSprint = false;
        IsJump = false;
        IsCrouch = false;
        IsHand = false;
        IsUse = false;
        IsToMenu = false;
        #endregion Reset - Game

        #region Reset - Menu
        IsToGame = false;
        #endregion Reset - Menu
    }

    #region Game - Move
    public event Action<Vector2> OnMoveStarted;
    public event Action<Vector2> OnMovePerformed;
    public event Action<Vector2> OnMoveCancelled;
    public Vector2 Direction { get; private set; } = Vector2.zero;
    private void MoveStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Direction = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
        OnMoveStarted?.Invoke(Direction);
    }

    private void MovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Direction = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
        OnMovePerformed?.Invoke(Direction);
    }

    private void MoveCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Direction = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
        OnMoveCancelled?.Invoke(Direction);
    }
    #endregion Game - Move

    #region Game - Rotate
    public event Action<Vector2> OnRotateStarted;
    public event Action<Vector2> OnRotatePerformed;
    public event Action<Vector2> OnRotateCancelled;
    public Vector2 MouseDelta { get; private set; } = Vector2.zero;
    private void RotateStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        MouseDelta = context.ReadValue<Vector2>();
        OnRotateStarted?.Invoke(MouseDelta);
    }

    private void RotatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        MouseDelta = context.ReadValue<Vector2>();
        OnRotatePerformed?.Invoke(MouseDelta);
    }

    private void RotateCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        MouseDelta = context.ReadValue<Vector2>();
        OnRotateCancelled?.Invoke(MouseDelta);
    }
    #endregion Game - Rotate

    #region Game - Sprint
    public event Action<bool> OnSprintStarted;
    public event Action<bool> OnSprintPerformed;
    public event Action<bool> OnSprintCancelled;
    public bool IsSprint { get; private set; } = false;
    public bool IsSprintDown { get; private set; } = false;
    public bool IsSprintUp { get; private set; } = false;
    private void SprintStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsSprint = true;
        OnSprintStarted?.Invoke(IsSprint);
        IsSprintDown = true;
    }

    private void SprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnSprintPerformed?.Invoke(IsSprint);
    }

    private void SprintCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsSprint = false;
        OnSprintCancelled?.Invoke(IsSprint);
        IsSprintUp = true;
    }
    #endregion Game - Sprint

    #region Game - Jump
    public event Action<bool> OnJumpStarted;
    public event Action<bool> OnJumpPerformed;
    public event Action<bool> OnJumpCancelled;
    public bool IsJump { get; private set; } = false;
    public bool IsJumpDown { get; private set; } = false;
    public bool IsJumpUp { get; private set; } = false;
    private void JumpStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsJump = true;
        OnJumpStarted?.Invoke(IsJump);
        IsJumpDown = true;
    }

    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnJumpPerformed?.Invoke(IsJump);
    }

    private void JumpCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsJump = false;
        OnJumpCancelled?.Invoke(IsJump);
        IsJumpUp = true;
    }
    #endregion Game - Jump

    #region Game - Crouch
    public event Action<bool> OnCrouchStarted;
    public event Action<bool> OnCrouchPerformed;
    public event Action<bool> OnCrouchCancelled;
    public bool IsCrouch { get; private set; } = false;
    public bool IsCrouchDown { get; private set; } = false;
    public bool IsCrouchUp { get; private set; } = false;
    private void CrouchStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsCrouch = true;
        OnCrouchStarted?.Invoke(IsCrouch);
        IsCrouchDown = true;
    }

    private void CrouchPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnCrouchPerformed?.Invoke(IsCrouch);
    }

    private void CrouchCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsCrouch = false;
        OnCrouchCancelled?.Invoke(IsCrouch);
        IsCrouchUp = true;
    }
    #endregion Game - Crouch

    #region Game - Hand
    public event Action<UnityEngine.InputSystem.InputAction.CallbackContext> OnHandStarted;
    public event Action<UnityEngine.InputSystem.InputAction.CallbackContext> OnHandPerformed;
    public event Action<UnityEngine.InputSystem.InputAction.CallbackContext> OnHandCancelled;
    public bool IsHand { get; private set; } = false;
    public bool IsHandDown { get; private set; } = false;
    public bool IsHandUp { get; private set; } = false;
    private void HandStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsHand = true;
        OnHandStarted?.Invoke(context);
        IsHandDown = true;
    }

    private void HandPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnHandPerformed?.Invoke(context);
    }

    private void HandCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsHand = false;
        OnHandCancelled?.Invoke(context);
        IsHandUp = true;
    }
    #endregion Game - Hand

    #region Game - Use
    public event Action<UnityEngine.InputSystem.InputAction.CallbackContext> OnUseStarted;
    public event Action<UnityEngine.InputSystem.InputAction.CallbackContext> OnUsePerformed;
    public event Action<UnityEngine.InputSystem.InputAction.CallbackContext> OnUseCancelled;
    public bool IsUse { get; private set; } = false;
    public bool IsUseDown { get; private set; } = false;
    public bool IsUseUp { get; private set; } = false;
    private void UseStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsUse = true;
        OnUseStarted?.Invoke(context);
        IsUseDown = true;
    }

    private void UsePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnUsePerformed?.Invoke(context);
    }

    private void UseCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsUse = false;
        OnUseCancelled?.Invoke(context);
        IsUseUp = true;
    }
    #endregion Game - Use

    #region Game - ToMenu
    public event Action<bool> OnToMenuStarted;
    public event Action<bool> OnToMenuPerformed;
    public event Action<bool> OnToMenuCancelled;
    public bool IsToMenu { get; private set; } = false;
    public bool IsToMenuDown { get; private set; } = false;
    public bool IsToMenuUp { get; private set; } = false;
    private void ToMenuStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsToMenu = true;
        OnToMenuStarted?.Invoke(IsToMenu);
        IsToMenuDown = true;
    }

    private void ToMenuPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnToMenuPerformed?.Invoke(IsToMenu);
    }

    private void ToMenuCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsToMenu = false;
        OnToMenuCancelled?.Invoke(IsToMenu);
        IsToMenuUp = true;
    }
    #endregion Game - ToMenu

    #region Menu - ToGame
    public event Action<bool> OnToGameStarted;
    public event Action<bool> OnToGamePerformed;
    public event Action<bool> OnToGameCancelled;
    public bool IsToGame { get; private set; } = false;
    public bool IsToGameDown { get; private set; } = false;
    public bool IsToGameUp { get; private set; } = false;
    private void ToGameStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsToGame = true;
        OnToGameStarted?.Invoke(IsToGame);
        IsToGameDown = true;
    }

    private void ToGamePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnToGamePerformed?.Invoke(IsToGame);
    }

    private void ToGameCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsToGame = false;
        OnToGameCancelled?.Invoke(IsToGame);
        IsToGameUp = true;
    }
    #endregion Menu - ToGame
}

public enum InputControllerMap {DISABLED = -1, MENU = 0, GAME = 1 }
