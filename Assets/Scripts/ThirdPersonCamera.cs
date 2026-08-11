using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 3f;
    [SerializeField] private float sensitivity = 100f;

    [Header("Follow Settings")]
    public bool smoothFollow = true; // default mode
    [SerializeField] private float followSpeed = 10f;  // base catch-up speed
    [SerializeField] private float maxLagDistance = 10f; // prevent trailing too far

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Toggle follow mode with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            smoothFollow = !smoothFollow;
            Debug.Log("Camera mode: " + (smoothFollow ? "Smooth Follow" : "Hard Lock"));
        }
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 targetOffset = rotation * new Vector3(0, height, -distance);
        Vector3 targetPos = player.position + targetOffset;

        if (smoothFollow)
        {
            float dist = Vector3.Distance(transform.position, targetPos);

            // Dynamic catch-up: faster if lagging far behind
            float dynamicSpeed = followSpeed;
            if (dist > distance)
                dynamicSpeed += (dist - distance);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref currentVelocity,
                1f / dynamicSpeed
            );

            // Clamp max lag
            if (dist > maxLagDistance)
                transform.position = targetPos;
        }
        else
        {
            // Hard lock mode
            transform.position = targetPos;
        }

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
