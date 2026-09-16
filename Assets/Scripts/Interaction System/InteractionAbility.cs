using Sirenix.OdinInspector;
using UnityEngine;

public class InteractionAbility : MonoBehaviour
{
    // For code pretaining to player interaction ability

    [SerializeField] private float interactDistance = 15f;

    private UIManager uiController; // reference to UI controller for interaction highlight UI
    private Collider currentTargetCol;
    private IInteractable currentTargetInteractable; // store interface reference
    private bool canInteract;
    private bool wasHitting;

    [ShowInInspector] public UIManager UIController => uiController;
    [ShowInInspector] public Collider CurrentTargetBoxCol => currentTargetCol;
    [ShowInInspector] public bool CanInteract => canInteract;

    private void Awake()
    {
        uiController = GetComponent<UIManager>();
    }

    private void Update()
    {
        HandleRaycast();
        HandleDistanceCheck(currentTargetCol);
        HandleInteractionCheck();

        //if (currentTargetBoxCol != null)
        //{
        //    Debug.Log($"currentTargetBoxCol found: {currentTargetBoxCol.gameObject.name}");
        //}
    }

    private void HandleRaycast()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        bool isHitting = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, LayerMask.GetMask("Interactable")))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);

            if (hit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable))
            {
                currentTargetCol = hit.collider;
                currentTargetInteractable = interactable;
                isHitting = true;
                Debug.Log("Raycast hit interactable");
            }
        }
        else
        {
            currentTargetCol = null;
            currentTargetInteractable = null;
        }

        if (isHitting)
        {
            uiController.AdjustBounds(currentTargetCol);
        }

        if (isHitting && !wasHitting)
        {
            uiController.ToggleCornerMarkers(true);
            //Debug.Log("Started hitting interactable");
        }
        else if (!isHitting && wasHitting)
        {
            uiController.ToggleCornerMarkers(false);
            //Debug.Log("Stopped hitting interactable");
        }

        wasHitting = isHitting;
    }

    private void HandleDistanceCheck(Collider target)
    {
        if (target == null) { canInteract = false; return; }
        canInteract = Vector3.Distance(transform.position, target.transform.position) <= interactDistance;
    }

    private void HandleInteractionCheck()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract && currentTargetInteractable != null)
        {
            Debug.Log($"Player Interacted with {currentTargetCol.gameObject.name}");
            currentTargetInteractable.Interact();
        }
    }
}
