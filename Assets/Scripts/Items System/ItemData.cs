using UnityEngine;

// Basic template for Item scriptable object

public enum ItemType { Material, Consumable, Deliverable, Weapon }

[CreateAssetMenu(menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public GameObject prefab;
    public Sprite icon;
    public int maxStack = 99;
}
