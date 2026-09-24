using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    [Header("Movement Settings")]
    private float gravity = -9.81f;
    private float maxFallSpeed = -30f;
    private float jumpForce = 8f;

    [SerializeField] private float moveSpeed = 50f;

    private float originalMoveSpeed;
    private float crouchSpeed = 3f;

    private float dashSpeed = 15f;
    private float dashDuration = 0.5f;
    private float dashCooldown = 0.3f;

    private float slideMultiplier = 1.5f;
    private float slideEndThreshold = 2f;
    private float slideDecay = 0.99f;
    private float slideSteerStrength = 0.5f;

    private float dashSlideBuffer = 0.15f;

    private float friction = 8f;

    private float airAcceleration = 3f;
    private float maxAirSpeed = 15f;

    [Header("References")]
    [SerializeField] private Animator animator;

    private CharacterController controller;

    private GameObject debugCanvas;

    private TextMeshProUGUI debugText_playerState;

    [SerializeField]
    private TextMeshProUGUI debugText_currentSpeed;

    [SerializeField]
    private TextMeshProUGUI debugText_currentState;

    private Vector3 velocity;
    private Vector3 inputDir;

    private bool isDashing;
    private bool isSliding;

    private float lastDashTime;

    public bool MovementEnabled { get; private set; } = true;

    private void Awake()
    {

        if (!debugCanvas)
            debugCanvas =
                GameObject.FindGameObjectWithTag("DebugCanvas");

        controller =
            GetComponent<CharacterController>();

        if (animator == null)
            animator =
                GetComponentInChildren<Animator>();

        originalMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        debugText_playerState =
            debugCanvas.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleDash();
        HandleCrouch();

        if (M_InputManager.Instance.SlideHeld && !isSliding)
        {
            if (!isDashing ||
                (Time.time > lastDashTime + dashSlideBuffer))
            {
                StartCoroutine(SlideCoroutine());
            }
        }

        if (controller.isGrounded &&
            M_InputManager.Instance.JumpPressed)
        {
            Jump();
        }

        HandleAirControl();
        ApplyGravity();
        ApplyFriction();

        controller.Move(
            velocity * Time.deltaTime);

        UpdateAnimator();
        UpdateDebugState();

        Vector3 horizontalVel =
            new Vector3(
                velocity.x,
                0,
                velocity.z);

        debugText_currentSpeed.text =
            $"Current speed: {horizontalVel.magnitude:F2}";
    }

    public void EnableMovement()
    {
        MovementEnabled = true;
    }

    public void DisableMovement()
    {
        MovementEnabled = false;
    }

    #region Movement

    private void HandleMovementInput()
    {
        if (!MovementEnabled)
            return;

        if (isDashing || isSliding)
            return;

        if (M_InputManager.Instance.MoveInput == null) 
            return;

        Vector2 moveInput = M_InputManager.Instance.MoveInput;

        float h = moveInput.x;
        float v = moveInput.y;

        Vector3 camForward =
            Camera.main.transform.forward;

        Vector3 camRight =
            Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        inputDir =
            (camForward * v + camRight * h)
            .normalized;

        if (inputDir.magnitude > 0.1f)
        {
            transform.rotation =
                Quaternion.LookRotation(inputDir);
        }

        Vector3 targetVel =
            inputDir * moveSpeed;

        velocity.x =
            Mathf.Lerp(
                velocity.x,
                targetVel.x,
                0.2f);

        velocity.z =
            Mathf.Lerp(
                velocity.z,
                targetVel.z,
                0.2f);
    }

    #endregion

    #region Jump

    private void Jump()
    {
        velocity.y = jumpForce;
    }

    private void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            velocity.y +=
                gravity * Time.deltaTime;

            velocity.y =
                Mathf.Max(
                    velocity.y,
                    maxFallSpeed);
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    #endregion

    #region Dash

    private void HandleDash()
    {
        if (!MovementEnabled)
            return;

        if (M_InputManager.Instance.DashPressed &&
            Time.time > lastDashTime + dashCooldown &&
            !isDashing)
        {
            isDashing = true;

            lastDashTime = Time.time;

            Vector3 dashImpulse =
                (inputDir.magnitude > 0.1f
                    ? inputDir
                    : transform.forward)
                * dashSpeed;

            velocity += dashImpulse;

            StartCoroutine(DashBuffer());
        }
    }

    private IEnumerator DashBuffer()
    {
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    #endregion

    #region Slide

    private IEnumerator SlideCoroutine()
    {
        isSliding = true;

        Vector3 horizontalVel =
            new Vector3(
                velocity.x,
                0,
                velocity.z);

        float currentSpeed =
            horizontalVel.magnitude *
            slideMultiplier;

        Vector3 slideDir =
            horizontalVel.normalized;

        while (
            currentSpeed > slideEndThreshold &&
            (
                M_InputManager.Instance.SlideHeld ||
                M_InputManager.Instance.CrouchHeld
            ))
        {
            Vector2 moveInput =
                M_InputManager.Instance.MoveInput;

            float h = moveInput.x;
            float v = moveInput.y;

            Vector3 camForward =
                Camera.main.transform.forward;

            Vector3 camRight =
                Camera.main.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 steeringInput =
                (camForward * v + camRight * h)
                .normalized;

            if (steeringInput.magnitude > 0.1f)
            {
                slideDir =
                    Vector3.Lerp(
                        slideDir,
                        steeringInput,
                        slideSteerStrength *
                        Time.deltaTime)
                    .normalized;
            }

            currentSpeed *= slideDecay;

            velocity =
                slideDir * currentSpeed +
                new Vector3(
                    0,
                    velocity.y,
                    0);

            if (controller.isGrounded &&
                M_InputManager.Instance.JumpPressed)
            {
                velocity =
                    slideDir * currentSpeed +
                    new Vector3(
                        0,
                        velocity.y,
                        0);

                Jump();

                break;
            }

            yield return null;
        }

        isSliding = false;
    }

    #endregion

    #region Crouch

    private void HandleCrouch()
    {
        if (M_InputManager.Instance.CrouchHeld)
        {
            controller.height = 1f;
            moveSpeed = crouchSpeed;
        }
        else
        {
            controller.height = 2f;
            moveSpeed = originalMoveSpeed;
        }
    }

    #endregion

    #region Air Control

    private void HandleAirControl()
    {
        if (controller.isGrounded)
            return;

        Vector2 moveInput =
            M_InputManager.Instance.MoveInput;

        float h = moveInput.x;
        float v = moveInput.y;

        Vector3 camForward =
            Camera.main.transform.forward;

        Vector3 camRight =
            Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 airInput =
            (camForward * v + camRight * h)
            .normalized;

        if (airInput.magnitude > 0.1f)
        {
            velocity +=
                airInput *
                airAcceleration *
                Time.deltaTime;

            Vector3 horizontalVel =
                new Vector3(
                    velocity.x,
                    0,
                    velocity.z);

            if (horizontalVel.magnitude > maxAirSpeed)
            {
                horizontalVel =
                    horizontalVel.normalized *
                    maxAirSpeed;

                velocity.x =
                    horizontalVel.x;

                velocity.z =
                    horizontalVel.z;
            }
        }
    }

    #endregion

    #region Friction

    private void ApplyFriction()
    {
        if (controller.isGrounded &&
            !isDashing &&
            !isSliding)
        {
            Vector3 horizontalVel =
                new Vector3(
                    velocity.x,
                    0,
                    velocity.z);

            horizontalVel =
                Vector3.Lerp(
                    horizontalVel,
                    Vector3.zero,
                    friction * Time.deltaTime);

            velocity.x = horizontalVel.x;
            velocity.z = horizontalVel.z;
        }
    }

    #endregion

    #region Animator

    private void UpdateAnimator()
    {
        if (!animator)
            return;

        animator.SetFloat(
            "Speed",
            inputDir.magnitude);

        animator.SetBool(
            "IsDashing",
            isDashing);

        animator.SetBool(
            "IsSliding",
            isSliding);

        animator.SetBool(
            "IsCrouching",
            controller.height < 2f);
    }

    #endregion

    #region Debug

    private void UpdateDebugState()
    {
        string state = "Idle";

        if (isSliding)
        {
            state = "Sliding";
        }
        else if (isDashing)
        {
            state = "Dashing";
        }
        else if (controller.height < 2f)
        {
            state = "Crouching";
        }
        else if (inputDir.magnitude > 0.1f)
        {
            state = "Walking";
        }

        debugText_playerState.text =
            $"STATE\n{state}";

        debugText_currentState.text =
            $"Current State: {state}";
    }

    #endregion
}