using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public static FirstPersonCamera Instance;

    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    private bool cameraLocked = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CharacterMotor.Instance.isFirstPerson = true;

        LockCamera();
    }

    private void Update()
    {
        // Toggle lock/unlock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cameraLocked)
                UnlockCamera();
            else
                LockCamera();
        }

        if (!cameraLocked)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void LockCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        cameraLocked = true;

        Debug.Log("First Person Camera Locked");
    }

    public void UnlockCamera()
    {
        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;

        cameraLocked = false;

        Debug.Log("First Person Camera Unlocked");
    }

    private void OnEnable()
    {
        if (CharacterMotor.Instance != null)
            CharacterMotor.Instance.isFirstPerson = true;

        LockCamera();
    }

    private void OnDisable()
    {
        if (CharacterMotor.Instance != null)
            CharacterMotor.Instance.isFirstPerson = false;
    }
}