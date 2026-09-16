using System.Collections.Generic;
using UnityEngine;

public class M_InventoryManager : MonoBehaviour
{
    // For receiving item pickup data, adding it to the inventory both visually and in player data

    public static M_InventoryManager Instance;
    private Dictionary<ItemData, int> items = new();

    void Awake() => Instance = this;

    public void AddItem(ItemData item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (!items.ContainsKey(item)) items[item] = 0;
            items[item]++;
            Debug.Log($"Picked up {item.itemName}, total: {items[item]}");
            //InventoryUI.Instance.Refresh(items);
        }
    }


}