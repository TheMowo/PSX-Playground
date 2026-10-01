using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public static FirstPersonCamera Instance;

    private float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation;
    private bool cameraLocked = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CharacterMotor.Instance.isFirstPerson = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        if (CharacterMotor.Instance != null)
            CharacterMotor.Instance.isFirstPerson = true;

        M_GameManager.PauseStateChanged += HandlePauseStateChanged;

        UnlockCamera();
    }

    private void OnDisable()
    {
        if (CharacterMotor.Instance != null)
            CharacterMotor.Instance.isFirstPerson = false;

        M_GameManager.PauseStateChanged -= HandlePauseStateChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (cameraLocked)
                UnlockCamera();
            else
                FixCamera();
        }

        if (cameraLocked)
            return;

        if (M_InputManager.Instance == null)
            return;

        Vector2 lookInput = M_InputManager.Instance.LookInput;

        float lookX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float lookY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * lookX);
    }

    private void HandlePauseStateChanged(bool paused)
    {
        if (paused)
            FixCamera();
        else
            UnlockCamera();
    }

    public void FixCamera()
    {
        cameraLocked = true;
    }

    public void UnlockCamera()
    {
        cameraLocked = false;
    }
}