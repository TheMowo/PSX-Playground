using Sirenix.OdinInspector;
using UnityEngine;

public class InteractionAbility : MonoBehaviour
{
    // For code pretaining to player interaction ability
    // Currently overlapping funtionality between this script and an older script: PlayerInteractor.cs

    [SerializeField] private float interactDistance = 15f;

    private InteractionUIController uiController;
    private BoxCollider currentTargetBoxCol;
    private bool canInteract;
    private bool wasHitting;

    [ShowInInspector] public InteractionUIController UIController => uiController;
    [ShowInInspector] public BoxCollider CurrentTargetBoxCol => currentTargetBoxCol;
    [ShowInInspector] public bool CanInteract => canInteract;

    private void Awake()
    {
        uiController = GetComponent<InteractionUIController>();
    }

    private void Update()
    {
        HandleRaycast();
        HandleDistanceCheck(currentTargetBoxCol);
        HandleInteractionCheck();
        if(currentTargetBoxCol != null)
        {
            Debug.Log($"currentTargetBoxCol found");
        }
    }

    // Raycast
    private void HandleRaycast()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        bool isHitting = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, LayerMask.GetMask("Interactable")))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);

            if (hit.collider.gameObject.TryGetComponent<IInteractable>(out _))
            {
                currentTargetBoxCol = (BoxCollider)hit.collider;
                isHitting = true;
                Debug.Log("isHitting");
            }
        }
        else
        {
            currentTargetBoxCol = null;
        }

        if (isHitting)
        {
            uiController.AdjustBounds(currentTargetBoxCol);
        }

        // Only toggle UI when state changes
        if (isHitting && !wasHitting)
        {
            uiController.ToggleCornerMarkers(true);
            Debug.Log("Started hitting interactable");
        }
        else if (!isHitting && wasHitting)
        {
            uiController.ToggleCornerMarkers(false);
            Debug.Log("Stopped hitting interactable");
        }

        wasHitting = isHitting;
    }

    private void HandleDistanceCheck(BoxCollider target)
    {
        if (target == null) { canInteract = false; return; }
        canInteract = Vector3.Distance(transform.position, target.transform.position) <= interactDistance;
    }

    private void HandleInteractionCheck()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract && currentTargetBoxCol)
        {
            Debug.Log($"Player Interacted with {currentTargetBoxCol.gameObject.name}");
            // Get reference of interaction object and run interact funtion here

        }
    }

    //Interact --> store ID
    //InventoryManager.Instance.AddItem(item);
    //Interactable.Destroy(gameObject);
}
