using Sirenix.OdinInspector;
using UnityEngine;

public class InteractionAbility : MonoBehaviour
{
    [SerializeField] private float interactDistance = 15f;

    [Title("Raycast")]
    [SerializeField] private LayerMask interactableLayers = -1;
    [SerializeField] private LayerMask ignoreLayers;

    [Header("Interaction")]
    [SerializeField] private float interactionBlockDuration = 0.25f;

    private M_UIManager uiController;
    private Collider currentTargetCol;
    private IInteractable currentTargetInteractable;

    private bool canInteract;
    private bool wasHitting;

    private float interactionBlockedUntil;

    [ShowInInspector] public M_UIManager UIController => uiController;
    [ShowInInspector] public Collider CurrentTargetBoxCol => currentTargetCol;
    [ShowInInspector] public bool CanInteract => canInteract;

    private void Awake()
    {
        uiController = GetComponent<M_UIManager>();
    }

    private void Start()
    {
        M_InputManager.Instance.InteractPerformed += OnInteractPressed;
    }

    private void OnDestroy()
    {
        if (!M_InputManager.HasInstance)
            return;

        M_InputManager.Instance.InteractPerformed -= OnInteractPressed;
    }

    private void Update()
    {
        HandleRaycast();
        HandleDistanceCheck(currentTargetCol);
    }

    public void BlockInteraction()
    {
        interactionBlockedUntil =
            Time.unscaledTime + interactionBlockDuration;
    }

    public void BlockInteraction(float duration)
    {
        interactionBlockedUntil =
            Mathf.Max(
                interactionBlockedUntil,
                Time.unscaledTime + duration);
    }

    private void OnInteractPressed()
    {
        Debug.Log("INTERACT EVENT RECEIVED");

        if (Time.unscaledTime < interactionBlockedUntil)
        {
            Debug.Log("Interaction blocked");
            return;
        }

        if (!canInteract)
        {
            Debug.Log("Cannot interact");
            return;
        }

        if (currentTargetInteractable == null)
        {
            Debug.Log("No interactable");
            return;
        }

        Debug.Log($"Player Interacted with {currentTargetCol.gameObject.name}");

        currentTargetInteractable.Interact();
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

            if ((ignoreLayers & hitLayerMask) != 0)
                continue;

            if ((interactableLayers & hitLayerMask) == 0)
                continue;

            Debug.DrawLine(ray.origin, hit.point, Color.green);

            if (!hit.collider.TryGetComponent<IInteractable>(out var interactable))
                continue;

            currentTargetCol = hit.collider;
            currentTargetInteractable = interactable;
            isHitting = true;

            break;
        }

        if (isHitting)
            uiController.AdjustBounds(currentTargetCol);

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
}
