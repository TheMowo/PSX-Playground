using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public ItemData item;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(item);
            Destroy(gameObject);
        }
    }
}
