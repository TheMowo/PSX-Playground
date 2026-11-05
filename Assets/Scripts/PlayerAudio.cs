using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterMotor))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Footsteps")]
    [SerializeField] private float footstepInterval = 0.45f;
    [SerializeField] private float footstepSpeedThreshold = 1f;

    [Header("Surface Detection")]
    [SerializeField] private float surfaceCheckDistance = 2f;
    [SerializeField] private LayerMask surfaceLayers = ~0;

    private CharacterController controller;
    private CharacterMotor motor;

    private SurfaceType currentSurface = SurfaceType.Concrete;

    private float footstepTimer;
    private bool wasGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        motor = GetComponent<CharacterMotor>();
    }

    private void Update()
    {
        UpdateSurface();
        HandleLanding();
        HandleFootsteps();
    }

    private void UpdateSurface()
    {
        if (!controller.isGrounded)
            return;

        if (!Physics.Raycast(
            transform.position + Vector3.up * 0.25f,
            Vector3.down,
            out RaycastHit hit,
            surfaceCheckDistance,
            surfaceLayers))
            return;

        if (hit.collider.TryGetComponent(out Surface surface))
            currentSurface = surface.surfaceType;
        else if (hit.collider.GetComponentInParent<Surface>() is Surface parentSurface)
            currentSurface = parentSurface.surfaceType;
    }

    private void HandleLanding()
    {
        bool grounded = controller.isGrounded;

        if (!wasGrounded && grounded)
            M_AudioManager.Instance.PlayPlayerLand(gameObject);

        wasGrounded = grounded;
    }

    private void HandleFootsteps()
    {
        if (!controller.isGrounded)
        {
            footstepTimer = 0f;
            return;
        }

        float speed = motor.GetHorizontalSpeed();

        if (speed < footstepSpeedThreshold)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer += Time.deltaTime;

        float interval = footstepInterval / Mathf.Max(speed / 12f, 0.5f);

        if (footstepTimer < interval)
            return;

        footstepTimer = 0f;

        M_AudioManager.Instance
            .PlayPlayerFootstep(gameObject, currentSurface);
    }

    public void PlayJump()
    {
        M_AudioManager.Instance.PlayPlayerJump(gameObject);
    }

    public void PlayDash()
    {
        M_AudioManager.Instance.PlayPlayerDash(gameObject);
    }

    public void PlayDamage()
    {
        M_AudioManager.Instance.PlayPlayerDamage(gameObject);
    }

    public void PlayDeath()
    {
        M_AudioManager.Instance.PlayPlayerDeath(gameObject);
    }
}