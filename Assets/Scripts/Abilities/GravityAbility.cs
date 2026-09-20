using UnityEngine;

public class GravityAbility : PlayerAbility
{
    [SerializeField]
    float gravity = -9.81f;

    [SerializeField]
    float maxFallSpeed = -30f;

    public override int Priority => 0;

    public override void Tick()
    {
        if (!state.IsGrounded)
        {
            state.Velocity.y +=
                gravity * Time.deltaTime;

            state.Velocity.y =
                Mathf.Max(state.Velocity.y,
                          maxFallSpeed);
        }
        else if (state.Velocity.y < 0)
        {
            state.Velocity.y = -2f;
        }
    }
}