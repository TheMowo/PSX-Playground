using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{
    [SerializeField] private List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    private int currentIndex = 0;

    public void Interact()
    {
        Debug.Log($"{name} interacted with");

        if (cameras == null || cameras.Count == 0)
        {
            Debug.LogWarning("No cameras assigned to Terminal.");
            return;
        }

        // Advance index and wrap around
        currentIndex = (currentIndex + 1) % cameras.Count;

        CinemachineCamera newCamera = cameras[currentIndex];
        CameraManager.Instance.SwitchCamera(newCamera);
    }
}
