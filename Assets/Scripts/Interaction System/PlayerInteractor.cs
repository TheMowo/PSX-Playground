using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    // For code pretaining to player interaction ability
    // Depreceated, replaced by InteractionAbility

    private GameObject playerCamObj;
    private Transform playerCam;
    [SerializeField] private float interactionDist = 3;

    [SerializeField] private InputActionReference inputActionReference;

    [SerializeField] private LayerMask layers;

    private void Start()
    {
        playerCamObj = M_CameraManager.Instance.PlayerCam.gameObject;
        playerCam = playerCamObj.transform;
    }

    private void OnEnable()
    {
        inputActionReference.action.Enable();
        inputActionReference.action.started += PlayerInteracted;
    }
    private void OnDisable()
    {
        inputActionReference.action.Disable();
        inputActionReference.action.started -= PlayerInteracted;
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