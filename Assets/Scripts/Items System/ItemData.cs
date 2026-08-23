using UnityEngine;

public enum ItemType { Material, Consumable, Weapon }

[CreateAssetMenu(menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public Sprite icon;
    public int maxStack = 99;
}
