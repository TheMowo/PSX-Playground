using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
    // For items that sit in the world space and can be picked up by the player

    [System.Serializable]
    public struct LootEntry
    {
        public ItemData item;
        public int quantity;
    }

    public LootEntry[] lootContents;

    public LootEntry[] GetLoot() => lootContents;

    public void Interact()
    {
        Debug.Log($"Interaction with {gameObject}");
    }
}
