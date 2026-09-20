using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKeycode = KeyCode.Space;
    [SerializeField] private KeyCode crouchKeycode = KeyCode.LeftControl;
    [SerializeField] private KeyCode dashKeycode = KeyCode.LeftShift;
    [SerializeField] private KeyCode slideKeycode = KeyCode.LeftAlt;

    [Header("Movement Settings")]
    private float gravity = -9.81f;
    //private float fallMultiplier = 1.1f;   // exponential factor
    private float maxFallSpeed = -30f;      // clamp terminal velocity
    private float jumpForce = 8f;
    [SerializeField] private float moveSpeed = 20f;
    private float originalMoveSpeed;
    private float crouchSpeed = 3f;
    private float dashSpeed = 15f;
    private float dashDuration = 0.5f;
    private float dashCooldown = 0.3f;
    private float slideMultiplier = 1.5f;
    private float slideEndThreshold = 2f; // momentum cutoff
    private float slideDecay = 0.99f; // slower decay for long carry
    private float slideSteerStrength = 0.5f; // how much input bends slide
    private float dashSlideBuffer = 0.15f;
    private float friction = 8f; // ground friction
    private float airAcceleration = 3f;
    private float maxAirSpeed = 15f;


    [Header("References")]
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private GameObject debugCanvas;
    private TextMeshProUGUI debugText_playerState;
    [SerializeField] private TextMeshProUGUI debugText_currentSpeed;
    [SerializeField] private TextMeshProUGUI debugText_currentState;

    private Vector3 velocity;
    private Vector3 inputDir;
    private bool isDashing;
    private bool isSliding;
    private float lastDashTime;

    void Awake()
    {
        if (!debugCanvas)
            debugCanvas = GameObject.FindGameObjectWithTag("DebugCanvas");

        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        originalMoveSpeed = moveSpeed;
    }

    void Start()
    {
        debugText_playerState = debugCanvas.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleDash();
        HandleCrouch();

        if (Input.GetKeyDown(slideKeycode) && !isSliding)
        {
            if (!isDashing || (Time.time > lastDashTime + dashSlideBuffer))
                StartCoroutine(SlideCoroutine());
        }

        if (controller.isGrounded && Input.GetKeyDown(jumpKeycode))
            Jump();

        HandleAirControl();   // NEW
        ApplyGravity();
        ApplyFriction();

        controller.Move(velocity * Time.deltaTime);

        UpdateAnimator();
        UpdateDebugState();

        Vector3 horizontalVel = new(velocity.x, 0, velocity.z);
        debugText_currentSpeed.text = ($"Current speed: {horizontalVel.magnitude}");
    }

    #region Movement
    void HandleMovementInput()
    {
        if (isDashing || isSliding) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        inputDir = (camForward * v + camRight * h).normalized;

        if (inputDir.magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(inputDir);

        // Apply movement input to velocity (horizontal only)
        Vector3 targetVel = inputDir * moveSpeed;
        velocity.x = Mathf.Lerp(velocity.x, targetVel.x, 0.2f);
        velocity.z = Mathf.Lerp(velocity.z, targetVel.z, 0.2f);
    }
    #endregion

    #region Jump
    void Jump()
    {
        velocity.y = jumpForce; // preserve horizontal momentum
    }

    void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, maxFallSpeed);
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; // keep grounded
        }

    }
    #endregion

    #region Dash
    void HandleDash()
    {
        if (Input.GetKeyDown(dashKeycode) && Time.time > lastDashTime + dashCooldown && !isDashing)
        {
            isDashing = true;
            lastDashTime = Time.time;

            // Momentum chaining: add impulse to existing velocity
            Vector3 dashImpulse = (inputDir.magnitude > 0.1f ? inputDir : transform.forward) * dashSpeed;
            velocity += dashImpulse;

            StartCoroutine(DashBuffer());
        }
    }


    IEnumerator DashBuffer()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
    #endregion

    #region Slide
    IEnumerator SlideCoroutine()
    {
        isSliding = true;

        Vector3 horizontalVel = new(velocity.x, 0, velocity.z);
        float currentSpeed = horizontalVel.magnitude * slideMultiplier;
        Vector3 slideDir = horizontalVel.normalized;

        while (currentSpeed > slideEndThreshold && (Input.GetKey(slideKeycode) || Input.GetKey(crouchKeycode)))
        {
            // Steering input
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0; camRight.y = 0;
            camForward.Normalize(); camRight.Normalize();

            Vector3 inputDir = (camForward * v + camRight * h).normalized;
            if (inputDir.magnitude > 0.1f)
                slideDir = Vector3.Lerp(slideDir, inputDir, slideSteerStrength * Time.deltaTime).normalized;

            // Exponential decay
            currentSpeed *= slideDecay;

            // Apply slide momentum (preserve vertical velocity)
            velocity = slideDir * currentSpeed + new Vector3(0, velocity.y, 0);

            // Jump cancel
            if (controller.isGrounded && Input.GetKeyDown(jumpKeycode))
            {
                velocity = slideDir * currentSpeed + new Vector3(0, velocity.y, 0);
                Jump();
                break;
            }

            yield return null;
        }

        isSliding = false;
    }


    #endregion

    #region Crouch
    void HandleCrouch()
    {
        if (Input.GetKey(crouchKeycode))
        {
            controller.height = 1.0f;
            moveSpeed = crouchSpeed;
        }
        else
        {
            controller.height = 2.0f;
            moveSpeed = originalMoveSpeed;
        }
    }
    #endregion

    #region Air Control
    void HandleAirControl()
    {
        if (controller.isGrounded) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 inputDir = (camForward * v + camRight * h).normalized;

        if (inputDir.magnitude > 0.1f)
        {
            velocity += airAcceleration * Time.deltaTime * inputDir;

            Vector3 horizontalVel = new(velocity.x, 0, velocity.z);
            if (horizontalVel.magnitude > maxAirSpeed)
            {
                horizontalVel = horizontalVel.normalized * maxAirSpeed;
                velocity.x = horizontalVel.x;
                velocity.z = horizontalVel.z;
            }
        }
    }
    #endregion

    #region Friction
    void ApplyFriction()
    {
        if (controller.isGrounded && !isDashing && !isSliding)
        {
            Vector3 horizontalVel = new(velocity.x, 0, velocity.z);
            horizontalVel = Vector3.Lerp(horizontalVel, Vector3.zero, friction * Time.deltaTime);
            velocity.x = horizontalVel.x;
            velocity.z = horizontalVel.z;
        }
    }
    #endregion

    #region Animator
    void UpdateAnimator()
    {
        if (!animator) return;

        animator.SetFloat("Speed", inputDir.magnitude);
        animator.SetBool("IsDashing", isDashing);
        animator.SetBool("IsSliding", isSliding);
        animator.SetBool("IsCrouching", controller.height < 2f);
    }
    #endregion

    #region Debug
    void UpdateDebugState()
    {
        string state = "Idle";

        if (isSliding)
            state = "Sliding";
        else if (isDashing)
            state = "Dashing";
        else if (controller.height < 2f)
            state = "Crouching";
        else if (inputDir.magnitude > 0.1f)
            state = "Walking";

        debugText_playerState.text = $"STATE\n{state}";
        debugText_currentState.text = $"Current State: {state}";
    }
    #endregion
}