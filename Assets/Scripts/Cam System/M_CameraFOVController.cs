using UnityEngine;
using Unity.Cinemachine;

public class M_CameraFOVController : MonoBehaviour
{
    [SerializeField] private float baseFOV = 90f;
    [SerializeField] private float maxFOV = 115f;
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private float lerpSpeed = 10f;

    private CharacterMotor motor;

    private void Start()
    {
        motor = CharacterMotor.Instance;
    }

    private void Update()
    {
        CinemachineCamera cam = M_CameraManager.Instance.ActiveCamera;

        if (cam == null || motor == null)
            return;

        float speed = motor.GetHorizontalSpeed();

        float targetFOV = Mathf.Clamp(baseFOV + speed * speedMultiplier, baseFOV, maxFOV);

        cam.Lens.FieldOfView = Mathf.Lerp(cam.Lens.FieldOfView, targetFOV, lerpSpeed * Time.deltaTime);
    }
}