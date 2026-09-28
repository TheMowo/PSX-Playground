using Unity.Cinemachine;
using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    [Header("Camera Objects")]
    public CinemachineCamera firstPersonCamera;
    public CinemachineCamera thirdPersonCamera;

    [Header("Controllers")]
    public FirstPersonCamera firstPersonLook;
    public ThirdPersonCamera thirdPersonLook;

    private bool isFirstPerson = true;

    private void Start()
    {
        SetFirstPerson();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isFirstPerson)
                SetThirdPerson();
            else
                SetFirstPerson();
        }
    }

    void SetFirstPerson()
    {
        isFirstPerson = true;

        firstPersonCamera.gameObject.SetActive(true);
        thirdPersonCamera.gameObject.SetActive(false);

        firstPersonLook.enabled = true;
        thirdPersonLook.enabled = false;
    }

    void SetThirdPerson()
    {
        isFirstPerson = false;

        firstPersonCamera.gameObject.SetActive(false);
        thirdPersonCamera.gameObject.SetActive(true);

        firstPersonLook.enabled = false;
        thirdPersonLook.enabled = true;
    }
}