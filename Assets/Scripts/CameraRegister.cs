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
        CameraManager.Instance.Unregister(GetComponent<CinemachineCamera>());
    }

    private void OnDestroy()
    {
        CameraManager.Instance.Unregister(GetComponent<CinemachineCamera>());
    }

    private IEnumerator OnEnableRegis()
    {
        yield return new WaitUntil(() => CameraManager.Instance != null);
        yield return new WaitForEndOfFrame();
        CameraManager.Instance.Register(GetComponent<CinemachineCamera>());
    }
}
