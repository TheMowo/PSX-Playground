using UnityEngine;

public class JumpAbility : PlayerAbility
{
    [SerializeField]
    float jumpForce = 8f;

    public override int Priority => 20;

    public override void Tick()
    {
        if (!state.IsGrounded)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            state.Velocity.y = jumpForce;
        }
    }
}