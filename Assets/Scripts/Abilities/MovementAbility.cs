using UnityEngine;

public class MovementAbility : PlayerAbility
{
    [SerializeField] float moveSpeed = 20f;

    public override int Priority => 10;

    public override void Tick()
    {
        if (state.IsDashing || state.IsSliding)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        state.InputDirection =
            (forward * v + right * h).normalized;

        if (state.InputDirection.magnitude > 0.1f)
        {
            transform.rotation =
                Quaternion.LookRotation(state.InputDirection);
        }

        Vector3 target =
            state.InputDirection * moveSpeed;

        state.Velocity.x =
            Mathf.Lerp(state.Velocity.x, target.x, 0.2f);

        state.Velocity.z =
            Mathf.Lerp(state.Velocity.z, target.z, 0.2f);
    }
}