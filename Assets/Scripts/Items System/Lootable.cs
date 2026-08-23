using UnityEngine;

public class Lootable : MonoBehaviour
{
    [System.Serializable]
    public struct LootEntry
    {
        public ItemData item;
        public int quantity;
    }

    public LootEntry[] lootContents;

    public LootEntry[] GetLoot() => lootContents;
}
