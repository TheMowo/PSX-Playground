using UnityEngine;

[CreateAssetMenu(menuName = "Deliveries/Delivery Data")]
public class DeliveryData : ScriptableObject
{
    public string deliveryName;

    [Header("Package")]
    public ItemData packageItem;

    [Header("Timing")]
    public float timeLimitSeconds = 120f;

    [Header("Rewards")]
    public int baseReward = 100;
    public int overtimePenaltyPerSecond = 1;
}