using Unity.Cinemachine;
using UnityEngine;

public class CameraRegister : MonoBehaviour
{
    private void OnEnable()
    {
        CameraManager.Instance.Register(GetComponent<CinemachineCamera>());
    }

    private void OnDisable()
    {
        CameraManager.Instance.Unregister(GetComponent<CinemachineCamera>());
    }
}
