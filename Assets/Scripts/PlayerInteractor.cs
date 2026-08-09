using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    private GameObject playerCamObj;
    private Transform playerCam;
    [SerializeField] private float interactionDist = 3;

    [SerializeField] private InputActionReference inputActionReference;

    [SerializeField] private LayerMask layers;

    private void Awake()
    {
        playerCamObj = CameraManager.Instance.PlayerCam.gameObject;
        playerCam = playerCamObj.transform;
    }

    private void OnEnable()
    {
        inputActionReference.action.Enable();
        inputActionReference.action.started += PlayerInteracted;
    }
    private void OnDisable()
    {
        
    }

    private void PlayerInteracted(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Ray ray = new Ray(playerCam.position, playerCam.forward);

            if (!Physics.Raycast(ray, out RaycastHit hitInfo, interactionDist)) return;

            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactableObj)) interactableObj.Interact();
        }
    }
}