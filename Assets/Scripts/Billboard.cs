using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // Get direction to camera
        Vector3 direction = mainCamera.transform.position - transform.position;

        // Zero out vertical difference (Y axis)
        direction.y = 0f;

        // If direction is not zero, rotate towards it
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
