using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class M_InputManager : Singleton<M_InputManager>,
    InputSystem_Actions.IPlayerActions,
    InputSystem_Actions.IUIActions
{
    public enum InputDeviceType
    {
        Mouse,
        Keyboard,
        Gamepad
    }

    public InputDeviceType LastInputDevice { get; private set; } = InputDeviceType.Mouse;

    public bool UsingMouse => LastInputDevice == InputDeviceType.Mouse;
    public bool UsingKeyboard => LastInputDevice == InputDeviceType.Keyboard;
    public bool UsingGamepad => LastInputDevice == InputDeviceType.Gamepad;

    public InputSystem_Actions Controls => controls;

    #region Events

    public event Action PausePressed;

    public event Action SubmitPerformed;
    public event Action CancelPerformed;

    public event Action PreviousPerformed;
    public event Action NextPerformed;

    public event Action<Vector2> NavigatePerformed;

    #endregion

    #region Player Inputs

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool ThrowPressed { get; private set; }

    public bool AttackPressed { get; private set; }
    public bool AltAttackPressed { get; private set; }
    public bool FirePressed { get; private set; }
    public bool InteractPressed { get; private set; }

    public bool SprintHeld { get; private set; }
    public bool CrouchHeld { get; private set; }
    public bool SlideHeld { get; private set; }

    #endregion

    #region UI Inputs

    public Vector2 NavigateInput { get; private set; }

    public bool SubmitPressed { get; private set; }
    public bool CancelPressed { get; private set; }

    #endregion

    private InputSystem_Actions controls;

    protected override void Awake()
    {
        base.Awake();

        controls = new InputSystem_Actions();

        controls.Player.SetCallbacks(this);
        controls.UI.SetCallbacks(this);
    }

    private void OnEnable()
    {
        controls?.Enable();
    }

    private void OnDisable()
    {
        controls?.Disable();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.delta.ReadValue() != Vector2.zero || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
        {
            LastInputDevice = InputDeviceType.Mouse;
        }
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        DashPressed = false;
        ThrowPressed = false;

        AttackPressed = false;
        AltAttackPressed = false;
        FirePressed = false;
        InteractPressed = false;

        SubmitPressed = false;
        CancelPressed = false;
    }

    private void SetDevice(InputAction.CallbackContext context)
    {
        if (!(context.performed || context.started))
            return;

        if (context.control.device is Gamepad)
            LastInputDevice = InputDeviceType.Gamepad;
        else if (context.control.device is Mouse)
            LastInputDevice = InputDeviceType.Mouse;
        else
            LastInputDevice = InputDeviceType.Keyboard;
    }

    #region Player

    public event Action InteractPerformed;

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        SetDevice(context);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
        SetDevice(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            JumpPressed = true;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            DashPressed = true;
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            ThrowPressed = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            AttackPressed = true;
    }

    public void OnAltAttack(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            AltAttackPressed = true;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            FirePressed = true;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (!context.performed)
            return;

        InteractPressed = true;
        InteractPerformed?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            SprintHeld = true;
        else if (context.canceled)
            SprintHeld = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            CrouchHeld = true;
        else if (context.canceled)
            CrouchHeld = false;
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (context.performed)
            SlideHeld = true;
        else if (context.canceled)
            SlideHeld = false;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (!context.performed)
            return;

        PausePressed?.Invoke();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (!context.performed)
            return;

        PreviousPerformed?.Invoke();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (!context.performed)
            return;

        NextPerformed?.Invoke();
    }

    #endregion

    #region UI

    public void OnNavigate(InputAction.CallbackContext context)
    {
        NavigateInput = context.ReadValue<Vector2>();

        SetDevice(context);

        if (context.performed)
            NavigatePerformed?.Invoke(NavigateInput);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (!context.performed)
            return;

        Debug.Log("Submit Performed");

        SubmitPressed = true;
        SubmitPerformed?.Invoke();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        SetDevice(context);

        if (!context.performed)
            return;

        CancelPressed = true;
        CancelPerformed?.Invoke();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        SetDevice(context);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        SetDevice(context);
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        SetDevice(context);
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        SetDevice(context);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        SetDevice(context);
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
    }

    #endregion
}