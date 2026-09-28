using UnityEngine;

public class DeliveryDestination : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;

    private void OnEnable()
    {
        mesh = GetComponent<MeshRenderer>();

        if (M_DeliveryManager.Instance != null)
        {
            M_DeliveryManager.Instance.OnDeliveryStarted += HandleDeliveryStarted;
            M_DeliveryManager.Instance.OnDeliveryCompleted += HandleDeliveryEnded;
            M_DeliveryManager.Instance.OnDeliveryCancelled += HandleDeliveryCancelled;
        }

        UpdateVisibility();
    }

    private void OnDisable()
    {
        if (M_DeliveryManager.Instance != null)
        {
            M_DeliveryManager.Instance.OnDeliveryStarted -= HandleDeliveryStarted;
            M_DeliveryManager.Instance.OnDeliveryCompleted -= HandleDeliveryEnded;
            M_DeliveryManager.Instance.OnDeliveryCancelled -= HandleDeliveryCancelled;
        }
    }

    private void HandleDeliveryStarted(ActiveDelivery delivery)
    {
        UpdateVisibility();
    }

    private void HandleDeliveryEnded(int reward)
    {
        SetVisible(false);
    }

    private void HandleDeliveryCancelled()
    {
        SetVisible(false);
    }

    private void UpdateVisibility()
    {
        Transform activeDestination =
            M_DeliveryManager.Instance?.GetCurrentDestination();

        SetVisible(activeDestination == transform);
    }

    private void SetVisible(bool visible)
    {
        if (mesh != null)
            mesh.enabled = visible;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Transform destination =
            M_DeliveryManager.Instance.GetCurrentDestination();

        if (destination == transform)
        {
            M_DeliveryManager.Instance.CompleteDelivery();
        }
    }
}