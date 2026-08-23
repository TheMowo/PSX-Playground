using System.Linq;
using UnityEngine;

[System.Serializable]
public struct WeightedDrop
{
    public ItemData item;
    public int weight;
}

[CreateAssetMenu(menuName = "WeightedDropTable")]
public class WeightedDropTable : ScriptableObject
{
    public WeightedDrop[] drops;

    public ItemData GetDrop()
    {
        int totalWeight = drops.Sum(d => d.weight);
        int roll = Random.Range(0, totalWeight);
        foreach (var d in drops)
        {
            roll -= d.weight;
            if (roll < 0) return d.item;
        }
        return null;
    }
}
