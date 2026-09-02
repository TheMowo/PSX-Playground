using UnityEngine;
using UnityEngine.UI;

public class InteractionUIController : MonoBehaviour
{
    [SerializeField] private RectTransform[] CornerMarkers = new RectTransform[4]; // Top Left, Top Right, Bottom Left, Bottom Right
    [SerializeField] private Canvas canvas; // assign your UI canvas in inspector

    private void Awake()
    {
        ToggleCornerMarkers(false);
    }

    public void AdjustBounds(BoxCollider boxCollider)
    {
        var (topLeft, topRight, bottomLeft, bottomRight) =
            BoundsCalculator.CalculateCanvasCorners(boxCollider, canvas);

        CornerMarkers[0].anchoredPosition = topLeft;
        CornerMarkers[1].anchoredPosition = topRight;
        CornerMarkers[2].anchoredPosition = bottomLeft;
        CornerMarkers[3].anchoredPosition = bottomRight;
    }

    public void ToggleCornerMarkers(bool active)
    {
        foreach (var cornerMarker in CornerMarkers)
        {
            cornerMarker.GetComponent<Image>().enabled = active;
        }
    }

    //// blah blah AI slop (not cleaned)
    //public GameObject lootPanel;
    //public Transform lootListParent;
    //public GameObject lootItemPrefab;

    //private WorldItem currentLoot;


    //public void OpenLoot(WorldItem lootable)
    //{
    //    currentLoot = lootable;
    //    lootPanel.SetActive(true);

    //    foreach (Transform child in lootListParent) Destroy(child.gameObject);

    //    foreach (var entry in lootable.GetLoot())
    //    {
    //        GameObject itemUI = Instantiate(lootItemPrefab, lootListParent);
    //        InventoryManager.Instance.AddItem(lootable.GetComponent<ItemData>());
    //    }
    //}

    //public void CloseLoot()
    //{
    //    lootPanel.SetActive(false);
    //    currentLoot = null;
    //}
}
