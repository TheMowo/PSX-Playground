using UnityEngine;

public class LootUI : MonoBehaviour
{
    public GameObject lootPanel;
    public Transform lootListParent;
    public GameObject lootItemPrefab;

    private Lootable currentLoot;

    public void OpenLoot(Lootable lootable)
    {
        currentLoot = lootable;
        lootPanel.SetActive(true);

        foreach (Transform child in lootListParent) Destroy(child.gameObject);

        foreach (var entry in lootable.GetLoot())
        {
            GameObject itemUI = Instantiate(lootItemPrefab, lootListParent);
            InventoryManager.Instance.AddItem(lootable.GetComponent<ItemData>());
        }
    }

    public void CloseLoot()
    {
        lootPanel.SetActive(false);
        currentLoot = null;
    }
}
