using UnityEngine;

[CreateAssetMenu(menuName = "DropTable")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public struct DropEntry
    {
        public ItemData item;
        [Range(0, 1)] public float dropChance;
    }
    public DropEntry[] drops;

    public ItemData GetDrop()
    {
        foreach (var entry in drops)
        {
            if (Random.value <= entry.dropChance) return entry.item;
        }
        return null;
    }
}
