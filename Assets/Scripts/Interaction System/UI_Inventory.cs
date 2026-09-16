using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    public static UI_Inventory Instance;

    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;

    void Awake() => Instance = this;

    public void Refresh(Dictionary<ItemData, int> items)
    {
        foreach (Transform child in slotParent) Destroy(child.gameObject);

        foreach (var kvp in items)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slot.GetComponentInChildren<UnityEngine.UI.Image>().sprite = kvp.Key.icon;
            slot.GetComponentInChildren<UnityEngine.UI.Text>().text = kvp.Key.itemName + " x" + kvp.Value;
        }
    }
}
