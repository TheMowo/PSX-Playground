using Unity.Cinemachine;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private bool useActiveCam;

    [Header("Rotation Limit")]
    [SerializeField] private bool restrictRotation;
    [SerializeField] private float maxAngleFromStart = 45f;

    private CinemachineCamera mainCamera;
    private CinemachineCamera activeCam;
    private bool isWorldCanvas;

    private float startingYRotation;

    void Start()
    {
        mainCamera = M_CameraManager.Instance.PlayerCam;

        if (GetComponent<Canvas>())
            isWorldCanvas = true;

        startingYRotation = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        Vector3 direction;

        if (useActiveCam)
        {
            activeCam = M_CameraManager.Instance.ActiveCamera;
            direction = activeCam.transform.position - transform.position;
        }
        else
        {
            direction = mainCamera.transform.position - transform.position;
        }

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = isWorldCanvas
                ? Quaternion.LookRotation(-direction)
                : Quaternion.LookRotation(direction);

            if (restrictRotation)
            {
                float targetY = rotation.eulerAngles.y;

                float delta = Mathf.DeltaAngle(startingYRotation, targetY);
                delta = Mathf.Clamp(delta, -maxAngleFromStart, maxAngleFromStart);

                rotation = Quaternion.Euler(0f, startingYRotation + delta, 0f);
            }

            transform.rotation = rotation;
        }
    }
}