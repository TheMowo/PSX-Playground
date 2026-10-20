using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    public static CharacterMotor Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float groundAcceleration = 25f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float maxFallSpeed = -40f;

    [Header("Air Control")]
    [SerializeField] private float airAcceleration = 35f;
    [SerializeField] private float maxAirSpeed = 20f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.3f;

    [Header("Slide")]
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float slideMultiplier = 1.8f;
    [SerializeField] private float slideDecay = 0.995f;
    [SerializeField] private float slideEndThreshold = 2f;
    [SerializeField] private float slideSteerStrength = 8f;

    [Header("Friction")]
    [SerializeField] private float friction = 4f;

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

    public bool isFirstPerson;

    public bool MovementEnabled { get; private set; } = true;

    private void Awake()
    {
        Instance = this;

        controller = GetComponent<CharacterController>();

        if (!debugCanvas)
            debugCanvas = GameObject.FindGameObjectWithTag("DebugCanvas");

        if (!animator)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (debugCanvas)
            debugText_playerState =
                debugCanvas.GetComponentInChildren<TextMeshProUGUI>();

        controller.height = standingHeight;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleDash();

        if (M_InputManager.Instance.SlideHeld &&
            !isSliding)
        {
            StartCoroutine(SlideCoroutine());
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

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z);

        if (debugText_currentSpeed)
        {
            debugText_currentSpeed.text =
                $"Current Speed: {horizontalVelocity.magnitude:F2}";
        }
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

        if (isSliding || isDashing)
            return;

        Vector2 moveInput =
            M_InputManager.Instance.MoveInput;

        Transform moveReference =
            isFirstPerson
                ? transform
                : Camera.main.transform;

        Vector3 forward =
            moveReference.forward;

        Vector3 right =
            moveReference.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        inputDir =
            (forward * moveInput.y +
             right * moveInput.x).normalized;

        if (!isFirstPerson &&
            inputDir.magnitude > 0.1f)
        {
            transform.rotation =
                Quaternion.LookRotation(inputDir);
        }

        if (controller.isGrounded)
        {
            Vector3 targetVelocity =
                inputDir * moveSpeed;

            velocity.x =
                Mathf.MoveTowards(
                    velocity.x,
                    targetVelocity.x,
                    groundAcceleration * moveSpeed * Time.deltaTime);

            velocity.z =
                Mathf.MoveTowards(
                    velocity.z,
                    targetVelocity.z,
                    groundAcceleration * moveSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region Jump

    private void Jump()
    {
        velocity.y = jumpForce;
    }

    #endregion

    #region Gravity

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
        else if (velocity.y < 0f)
        {
            velocity.y = -2f;
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

        Transform moveReference =
            isFirstPerson
                ? transform
                : Camera.main.transform;

        Vector3 forward =
            moveReference.forward;

        Vector3 right =
            moveReference.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 airInput =
            (forward * moveInput.y +
             right * moveInput.x).normalized;

        if (airInput.magnitude < 0.1f)
            return;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z);

        horizontalVelocity +=
            airInput *
            airAcceleration *
            Time.deltaTime;

        if (horizontalVelocity.magnitude >
            maxAirSpeed)
        {
            horizontalVelocity =
                horizontalVelocity.normalized *
                maxAirSpeed;
        }

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    #endregion

    #region Dash

    private void HandleDash()
    {
        if (!MovementEnabled)
            return;

        if (!M_InputManager.Instance.DashPressed)
            return;

        if (isDashing)
            return;

        if (Time.time <
            lastDashTime + dashCooldown)
            return;

        lastDashTime = Time.time;
        isDashing = true;

        Vector3 dashDirection =
            inputDir.magnitude > 0.1f
                ? inputDir
                : transform.forward;

        velocity +=
            dashDirection *
            dashSpeed;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(
            dashDuration);

        isDashing = false;
    }

    #endregion

    #region Slide

    private IEnumerator SlideCoroutine()
    {
        isSliding = true;

        controller.height = slideHeight;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z);

        float currentSpeed =
            Mathf.Max(
                horizontalVelocity.magnitude,
                moveSpeed);

        currentSpeed *= slideMultiplier;

        Vector3 slideDir =
            horizontalVelocity.sqrMagnitude > 0.1f
                ? horizontalVelocity.normalized
                : transform.forward;

        while (
            currentSpeed > slideEndThreshold &&
            M_InputManager.Instance.SlideHeld)
        {
            Transform moveReference =
                isFirstPerson
                    ? transform
                    : Camera.main.transform;

            Vector2 moveInput =
                M_InputManager.Instance.MoveInput;

            Vector3 forward =
                moveReference.forward;

            Vector3 right =
                moveReference.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 steeringInput =
                (forward * moveInput.y +
                 right * moveInput.x).normalized;

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
                Vector3.up * velocity.y;

            if (controller.isGrounded &&
                M_InputManager.Instance.JumpPressed)
            {
                velocity =
                    slideDir *
                    (currentSpeed + 5f);

                Jump();
                break;
            }

            yield return null;
        }

        controller.height = standingHeight;

        isSliding = false;
    }

    #endregion

    #region Friction

    private void ApplyFriction()
    {
        if (!controller.isGrounded)
            return;

        if (isSliding || isDashing)
            return;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z);

        horizontalVelocity =
            Vector3.Lerp(
                horizontalVelocity,
                Vector3.zero,
                friction * Time.deltaTime);

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
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
    }

    #endregion

    #region Debug

    private void UpdateDebugState()
    {
        string state = "Idle";

        if (isSliding)
            state = "Sliding";
        else if (isDashing)
            state = "Dashing";
        else if (inputDir.magnitude > 0.1f)
            state = "Moving";

        if (debugText_playerState)
        {
            debugText_playerState.text =
                $"STATE\n{state}";
        }

        if (debugText_currentState)
        {
            debugText_currentState.text =
                $"Current State: {state}";
        }
    }

    #endregion

    public void Launch(float verticalVelocity)
    {
        velocity.y = verticalVelocity;
    }
}