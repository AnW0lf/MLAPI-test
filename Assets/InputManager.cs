// GENERATED AUTOMATICALLY FROM 'Assets/InputManager.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputManager : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputManager()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputManager"",
    ""maps"": [
        {
            ""name"": ""Game"",
            ""id"": ""e6f98297-9a87-4c2c-a955-78aaea267d11"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""95189a38-97f4-43ea-81e0-f85a7e8fdca3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""01659258-d5f5-4034-8b2c-f8f7dd55b097"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""afd33394-0425-48d9-9894-82e6ffc940fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""5006ff13-45c0-4d26-832c-7614a00484bc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""318783f5-db54-45d0-a49d-7473d9e562a5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Hand"",
                    ""type"": ""Button"",
                    ""id"": ""60de9c4d-a1cb-4ee7-973d-0a340e51c6be"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""3028bb4c-892b-4e5e-9f92-6d6874a7f042"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Use"",
                    ""type"": ""Button"",
                    ""id"": ""1d63bc05-ccea-4771-81c0-2c1e728c26d4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToMenu"",
                    ""type"": ""Button"",
                    ""id"": ""31a3851b-eccd-4129-864d-f07360fedc52"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Letters"",
                    ""id"": ""50191b30-39f4-432e-bb2b-6021f1d56253"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""364c48f8-d129-40e0-a8c1-07f18444e1ed"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1a2ead8b-2466-4646-8158-872e0f6f4b27"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b3371b1e-e71a-4e91-b1e2-16f01ce20e60"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""35c21302-d0cb-42c3-aacf-6485dd096b1f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""ea6fe0d9-ae26-47f8-a465-4e62e4863009"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0944314e-9bd3-4b06-9f2a-57e2da88dd58"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""297a6b8d-3c6e-430f-906d-90eccb36ebfd"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea4936d3-ed9c-4764-9420-29cf9ff34b5e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2892d102-c282-489e-af8f-978b06910364"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hand"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b20cb954-ac85-46ca-9746-eab7150390a6"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42528ff8-aefb-4d98-a7ef-5292ed074884"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.05,y=0.05)"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d555934-c7e5-45d6-b9c4-63159c9489dd"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""52d62e28-59f1-4153-85ea-52e2e5f8ecc9"",
            ""actions"": [
                {
                    ""name"": ""ToGame"",
                    ""type"": ""Button"",
                    ""id"": ""9d2543d1-d172-4b58-9106-a3b800814530"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""66a63ebf-b756-431f-80ea-4afba624a272"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Game
        m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
        m_Game_Move = m_Game.FindAction("Move", throwIfNotFound: true);
        m_Game_Rotate = m_Game.FindAction("Rotate", throwIfNotFound: true);
        m_Game_Sprint = m_Game.FindAction("Sprint", throwIfNotFound: true);
        m_Game_Jump = m_Game.FindAction("Jump", throwIfNotFound: true);
        m_Game_Crouch = m_Game.FindAction("Crouch", throwIfNotFound: true);
        m_Game_Hand = m_Game.FindAction("Hand", throwIfNotFound: true);
        m_Game_Aim = m_Game.FindAction("Aim", throwIfNotFound: true);
        m_Game_Use = m_Game.FindAction("Use", throwIfNotFound: true);
        m_Game_ToMenu = m_Game.FindAction("ToMenu", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_ToGame = m_Menu.FindAction("ToGame", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Game
    private readonly InputActionMap m_Game;
    private IGameActions m_GameActionsCallbackInterface;
    private readonly InputAction m_Game_Move;
    private readonly InputAction m_Game_Rotate;
    private readonly InputAction m_Game_Sprint;
    private readonly InputAction m_Game_Jump;
    private readonly InputAction m_Game_Crouch;
    private readonly InputAction m_Game_Hand;
    private readonly InputAction m_Game_Aim;
    private readonly InputAction m_Game_Use;
    private readonly InputAction m_Game_ToMenu;
    public struct GameActions
    {
        private @InputManager m_Wrapper;
        public GameActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Game_Move;
        public InputAction @Rotate => m_Wrapper.m_Game_Rotate;
        public InputAction @Sprint => m_Wrapper.m_Game_Sprint;
        public InputAction @Jump => m_Wrapper.m_Game_Jump;
        public InputAction @Crouch => m_Wrapper.m_Game_Crouch;
        public InputAction @Hand => m_Wrapper.m_Game_Hand;
        public InputAction @Aim => m_Wrapper.m_Game_Aim;
        public InputAction @Use => m_Wrapper.m_Game_Use;
        public InputAction @ToMenu => m_Wrapper.m_Game_ToMenu;
        public InputActionMap Get() { return m_Wrapper.m_Game; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
        public void SetCallbacks(IGameActions instance)
        {
            if (m_Wrapper.m_GameActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                @Rotate.started -= m_Wrapper.m_GameActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnRotate;
                @Sprint.started -= m_Wrapper.m_GameActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnSprint;
                @Jump.started -= m_Wrapper.m_GameActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnJump;
                @Crouch.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCrouch;
                @Hand.started -= m_Wrapper.m_GameActionsCallbackInterface.OnHand;
                @Hand.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnHand;
                @Hand.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnHand;
                @Aim.started -= m_Wrapper.m_GameActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnAim;
                @Use.started -= m_Wrapper.m_GameActionsCallbackInterface.OnUse;
                @Use.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnUse;
                @Use.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnUse;
                @ToMenu.started -= m_Wrapper.m_GameActionsCallbackInterface.OnToMenu;
                @ToMenu.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnToMenu;
                @ToMenu.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnToMenu;
            }
            m_Wrapper.m_GameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Hand.started += instance.OnHand;
                @Hand.performed += instance.OnHand;
                @Hand.canceled += instance.OnHand;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Use.started += instance.OnUse;
                @Use.performed += instance.OnUse;
                @Use.canceled += instance.OnUse;
                @ToMenu.started += instance.OnToMenu;
                @ToMenu.performed += instance.OnToMenu;
                @ToMenu.canceled += instance.OnToMenu;
            }
        }
    }
    public GameActions @Game => new GameActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_ToGame;
    public struct MenuActions
    {
        private @InputManager m_Wrapper;
        public MenuActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToGame => m_Wrapper.m_Menu_ToGame;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @ToGame.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnToGame;
                @ToGame.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnToGame;
                @ToGame.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnToGame;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ToGame.started += instance.OnToGame;
                @ToGame.performed += instance.OnToGame;
                @ToGame.canceled += instance.OnToGame;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    public interface IGameActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnHand(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnUse(InputAction.CallbackContext context);
        void OnToMenu(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnToGame(InputAction.CallbackContext context);
    }
}
