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
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallMultiplier = 1.1f;   // exponential factor
    [SerializeField] private float maxFallSpeed = -30f;      // clamp terminal velocity
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 0.8f;
    [SerializeField] private float slideMultiplier = 1.5f;
    [SerializeField] private float slideEndThreshold = 2f; // momentum cutoff
    [SerializeField] private float dashSlideBuffer = 0.15f;
    [SerializeField] private float friction = 8f; // ground friction

    [Header("References")]
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private GameObject debugCanvas;
    private TextMeshProUGUI debugText_playerState;

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

        ApplyGravity();
        ApplyFriction();

        controller.Move(velocity * Time.deltaTime);

        UpdateAnimator();
        UpdateDebugState();
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
        velocity.x = inputDir.x * moveSpeed;
        velocity.z = inputDir.z * moveSpeed;
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
            // If already falling, apply exponential growth
            if (velocity.y < 0)
            {
                velocity.y *= fallMultiplier;
                velocity.y = Mathf.Max(velocity.y, maxFallSpeed); // clamp
            }
            else
            {
                // Initial jump arc still uses normal gravity pull
                velocity.y += gravity * Time.deltaTime;
            }
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
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

            // Preserve current momentum and add dash impulse
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
        Vector3 slideDir = transform.forward;
        float currentSpeed = velocity.magnitude * slideMultiplier;

        while (currentSpeed > slideEndThreshold && Input.GetKey(slideKeycode))
        {
            // Exponential decay
            currentSpeed *= 0.95f;

            // Apply slide momentum
            velocity = slideDir * currentSpeed + new Vector3(0, velocity.y, 0);

            // Jump cancel check
            if (controller.isGrounded && Input.GetKeyDown(jumpKeycode))
            {
                // Preserve horizontal momentum into jump
                velocity = slideDir * currentSpeed;
                Jump();
                break; // exit slide
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
            moveSpeed = 6f;
        }
    }
    #endregion

    #region Friction
    void ApplyFriction()
    {
        if (controller.isGrounded && !isDashing && !isSliding)
        {
            Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);
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

        if (isDashing)
            state = "Dashing";
        else if (isSliding)
            state = "Sliding";
        else if (controller.height < 2f)
            state = "Crouching";
        else if (inputDir.magnitude > 0.1f)
            state = "Walking";

        debugText_playerState.text = $"STATE\n{state}";
    }
    #endregion
}