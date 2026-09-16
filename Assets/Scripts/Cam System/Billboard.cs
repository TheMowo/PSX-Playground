using Unity.Cinemachine;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private bool useActiveCam;
    private CinemachineCamera mainCamera;
    private CinemachineCamera activeCam;
    private bool isWorldCanvas = false;

    void Start()
    {
        mainCamera = M_CameraManager.Instance.PlayerCam;

        if (GetComponent<Canvas>())
            isWorldCanvas = true;
    }

    void LateUpdate()
    {
        Vector3 direction;
        if (useActiveCam)
        {
            activeCam = M_CameraManager.Instance.ActiveCamera;
            // Get direction to camera
            direction = activeCam.transform.position - transform.position;
        }
        else
        {
            direction = mainCamera.transform.position - transform.position;
        }

        // Zero out vertical difference (Y axis)
        direction.y = 0f;

        // If direction is not zero, rotate towards it
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            if (isWorldCanvas)
            {
                // Invert the facing direction for world-space canvases
                rotation = Quaternion.LookRotation(-direction);
            }
            transform.rotation = rotation;
        }
    }
}
