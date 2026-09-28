using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public static ThirdPersonCamera Instance;

    [SerializeField] private Transform player;
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 3f;
    [SerializeField] private float sensitivity = 100f;

    [Header("Follow Settings")]
    public bool smoothFollow = true; 
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float maxLagDistance = 10f;

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    [SerializeField] private TextMeshProUGUI debugText_camLockState;

    private bool cameraLocked = false;

    private void Awake()
    {
        LockCamera();

        UpdateDebugText();
    }

    private void Start()
    {
        Instance = this;
    }

    void Update()
    {
        // Toggle follow mode with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            smoothFollow = !smoothFollow;
            Debug.Log("Camera mode: " + (smoothFollow ? "Smooth Follow" : "Hard Lock"));
            UpdateDebugText();
        }

        // Toggle lock/unlock with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cameraLocked) UnlockCamera();
            else LockCamera();
        }
    }

    void LateUpdate()
    {
        if (cameraLocked)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -30f, 60f);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 targetOffset = rotation * new Vector3(0, height, -distance);
        Vector3 targetPos = player.position + targetOffset;

        if (smoothFollow)
        {
            float dist = Vector3.Distance(transform.position, targetPos);
            float dynamicSpeed = followSpeed;
            if (dist > distance) dynamicSpeed += (dist - distance);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref currentVelocity,
                1f / dynamicSpeed
            );

            if (dist > maxLagDistance)
                transform.position = targetPos;
        }
        else
        {
            transform.position = targetPos;
        }

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }

    // Explicit lock/unlock methods
    public void LockCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraLocked = true;
        Debug.Log("Camera locked");
    }

    public void UnlockCamera()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraLocked = false;
        Debug.Log("Camera unlocked");
    }

    private void UpdateDebugText()
    {
        if (debugText_camLockState != null)
        {
            debugText_camLockState.text = $"Fixed cam: {!smoothFollow}";
        }
    }
}
