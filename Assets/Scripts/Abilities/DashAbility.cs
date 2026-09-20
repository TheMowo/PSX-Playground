using UnityEngine;
using System.Collections;

public class DashAbility : PlayerAbility
{
    [SerializeField]
    float dashSpeed = 15f;

    [SerializeField]
    float dashDuration = 0.5f;

    [SerializeField]
    float cooldown = 0.3f;

    float lastDashTime;

    public override int Priority => 30;

    public override void Tick()
    {
        if (state.IsDashing)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Time.time < lastDashTime + cooldown)
                return;

            lastDashTime = Time.time;

            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        state.IsDashing = true;

        Vector3 direction =
            state.InputDirection.magnitude > 0.1f
                ? state.InputDirection
                : transform.forward;

        state.Velocity += direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);
    }
}