using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    void Awake() => Instance = this;

    public void AddItem(ItemData item)
    {
        if (!items.ContainsKey(item)) items[item] = 0;
        items[item]++;
        Debug.Log($"Picked up {item.itemName}, total: {items[item]}");
    }
}