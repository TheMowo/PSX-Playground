using UnityEngine;

public class M_InputManager : MonoBehaviour
{
    public static M_InputManager Instance { get; private set; }


    // Player Movement
    public Vector2 MoveInput { get; private set; }

    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }

    public bool CrouchHeld { get; private set; }
    public bool SlideHeld { get; private set; }

    public bool ThrowPressed { get; private set; }


    // UI Navigation
    public Vector2 NavigateInput { get; private set; }

    public bool SubmitPressed { get; private set; }
    public bool CancelPressed { get; private set; }


    private InputSystem_Actions controls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Enable();

        // Player
        controls.Player.Move.performed += ctx =>
            MoveInput = ctx.ReadValue<Vector2>();

        controls.Player.Move.canceled += ctx =>
            MoveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx =>
            JumpPressed = true;

        controls.Player.Dash.performed += ctx =>
            DashPressed = true;

        controls.Player.Crouch.performed += ctx =>
            CrouchHeld = true;

        controls.Player.Crouch.canceled += ctx =>
            CrouchHeld = false;

        controls.Player.Slide.performed += ctx =>
            SlideHeld = true;

        controls.Player.Slide.canceled += ctx =>
            SlideHeld = false;

        controls.Player.Throw.performed += ctx =>
            ThrowPressed = true;

        // UI
        controls.UI.Navigate.performed += ctx =>
            NavigateInput = ctx.ReadValue<Vector2>();

        controls.UI.Navigate.canceled += ctx =>
            NavigateInput = Vector2.zero;

        controls.UI.Submit.performed += ctx =>
            SubmitPressed = true;

        controls.UI.Cancel.performed += ctx =>
            CancelPressed = true;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        DashPressed = false;
        ThrowPressed = false;

        SubmitPressed = false;
        CancelPressed = false;
    }
}