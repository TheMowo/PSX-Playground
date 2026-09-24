using UnityEngine;

public class ActiveDelivery
{
    public DeliveryData Data;
    public Transform Destination;

    public float TimeRemaining;
    public float Overtime;

    public bool IsOvertime;

    public ActiveDelivery(DeliveryData data, Transform destination)
    {
        Data = data;
        Destination = destination;
        TimeRemaining = data.timeLimitSeconds;
    }
}