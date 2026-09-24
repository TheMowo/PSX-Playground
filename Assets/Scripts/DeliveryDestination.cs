using UnityEngine;

public class DeliveryDestination : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Transform destination = M_DeliveryManager.Instance.GetCurrentDestination();

        if (destination == transform)
        {
            M_DeliveryManager.Instance.CompleteDelivery();
        }
    }


}