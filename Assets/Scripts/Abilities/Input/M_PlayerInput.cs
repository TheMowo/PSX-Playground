using UnityEngine;
using UnityEngine.InputSystem;

public class M_PlayerInput : MonoBehaviour
{
    public InputState InputState { get; private set; } = new();

    public void OnMove(InputAction.CallbackContext ctx)
    {
        InputState.Move = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            InputState.JumpPressed = true;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            InputState.DashPressed = true;
    }

    public void OnSlide(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            InputState.SlidePressed = true;
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        InputState.CrouchHeld = ctx.ReadValueAsButton();
    }

    public void ClearFrameInput()
    {
        InputState.JumpPressed = false;
        InputState.DashPressed = false;
        InputState.SlidePressed = false;
    }
}
