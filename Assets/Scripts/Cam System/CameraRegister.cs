using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraRegister : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(OnEnableRegis());
    }

    private void OnDisable()
    {
        M_CameraManager.Instance.Unregister(GetComponent<CinemachineCamera>());
    }

    private void OnDestroy()
    {
        M_CameraManager.Instance.Unregister(GetComponent<CinemachineCamera>());
    }

    private IEnumerator OnEnableRegis()
    {
        yield return new WaitUntil(() => M_CameraManager.Instance != null);
        yield return new WaitForEndOfFrame();
        M_CameraManager.Instance.Register(GetComponent<CinemachineCamera>());
    }
}
