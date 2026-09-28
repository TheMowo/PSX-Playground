using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;

    private bool firstPerson = false;

    private void Start()
    {
        SetThirdPerson();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (firstPerson)
                SetThirdPerson();
            else
                SetFirstPerson();
        }
    }

    void SetFirstPerson()
    {
        firstPerson = true;

        firstPersonCam.Priority = 20;
        thirdPersonCam.Priority = 10;
    }

    void SetThirdPerson()
    {
        firstPerson = false;

        firstPersonCam.Priority = 10;
        thirdPersonCam.Priority = 20;
    }
}