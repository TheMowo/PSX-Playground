using System.Collections.Generic;
using UnityEngine;

public class M_InventoryManager : MonoBehaviour
{
    public static M_InventoryManager Instance;

    private Dictionary<ItemData, int> items = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return;

        if (!items.ContainsKey(item))
            items[item] = 0;

        items[item] += amount;

        Debug.Log($"Added {amount}x {item.itemName}. Total: {items[item]}");

        // InventoryUI.Instance.Refresh(items);
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        if (!items.TryGetValue(item, out int currentAmount))
        {
            Debug.LogWarning($"Tried removing {item.itemName}, but player doesn't have any.");
            return false;
        }

        if (currentAmount < amount)
        {
            Debug.LogWarning(
                $"Tried removing {amount}x {item.itemName}, but only {currentAmount} available.");

            return false;
        }

        currentAmount -= amount;

        if (currentAmount <= 0)
            items.Remove(item);
        else
            items[item] = currentAmount;

        Debug.Log($"Removed {amount}x {item.itemName}");

        // InventoryUI.Instance.Refresh(items);

        return true;
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        return items.TryGetValue(item, out int currentAmount)
            && currentAmount >= amount;
    }

    public int GetItemCount(ItemData item)
    {
        if (item == null)
            return 0;

        return items.TryGetValue(item, out int count)
            ? count
            : 0;
    }

    public IReadOnlyDictionary<ItemData, int> GetItems()
    {
        return items;
    }
}