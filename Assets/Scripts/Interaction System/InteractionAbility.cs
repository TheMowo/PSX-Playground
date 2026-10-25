using Sirenix.OdinInspector;
using UnityEngine;

public class InteractionAbility : MonoBehaviour
{
    // For code pertaining to player interaction ability

    [SerializeField] private float interactDistance = 15f;

    [Title("Raycast")]
    [SerializeField] private LayerMask interactableLayers = -1;
    [SerializeField] private LayerMask ignoreLayers;

    private M_UIManager uiController; // reference to UI controller for interaction highlight UI
    private Collider currentTargetCol;
    private IInteractable currentTargetInteractable; // store interface reference
    private bool canInteract;
    private bool wasHitting;

    [ShowInInspector] public M_UIManager UIController => uiController;
    [ShowInInspector] public Collider CurrentTargetBoxCol => currentTargetCol;
    [ShowInInspector] public bool CanInteract => canInteract;

    private void Awake()
    {
        uiController = GetComponent<M_UIManager>();
    }

    private void Update()
    {
        HandleRaycast();
        HandleDistanceCheck(currentTargetCol);
        HandleInteractionCheck();

        //if (currentTargetCol != null)
        //{
        //    Debug.Log($"currentTargetCol found: {currentTargetCol.gameObject.name}");
        //}
    }

    private void HandleRaycast()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        bool isHitting = false;

        RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        currentTargetCol = null;
        currentTargetInteractable = null;

        foreach (RaycastHit hit in hits)
        {
            int hitLayerMask = 1 << hit.collider.gameObject.layer;

            // Skip ignored layers and continue checking further hits.
            if ((ignoreLayers & hitLayerMask) != 0)
            {
                continue;
            }

            // Skip anything not on an interactable layer.
            if ((interactableLayers & hitLayerMask) == 0)
            {
                continue;
            }

            Debug.DrawLine(ray.origin, hit.point, Color.green);

            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                currentTargetCol = hit.collider;
                currentTargetInteractable = interactable;
                isHitting = true;

                Debug.Log("Raycast hit interactable");
                break;
            }
        }

        if (isHitting)
        {
            uiController.AdjustBounds(currentTargetCol);
        }

        if (isHitting && !wasHitting)
        {
            uiController.ToggleCornerMarkers(true);
        }
        else if (!isHitting && wasHitting)
        {
            uiController.ToggleCornerMarkers(false);
        }

        wasHitting = isHitting;
    }

    private void HandleDistanceCheck(Collider target)
    {
        if (target == null)
        {
            canInteract = false;
            return;
        }

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