using UnityEngine;

public class ObjectiveMarker : MonoBehaviour
{
    private void Update()
    {
        Transform target =
            M_DeliveryManager.Instance
            .GetCurrentDestination();

        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        transform.position =
            target.position + Vector3.up * 3f;
    }
}