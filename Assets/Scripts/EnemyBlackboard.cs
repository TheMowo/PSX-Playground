using UnityEngine;

public class EnemyBlackboard : MonoBehaviour // Stores Data
{
    [Header("Target")]
    public Transform CurrentTarget;

    public bool HasSeenTarget;
    public bool CanSeeTarget;

    public Vector3 LastSeenPosition;
    public float LastSeenTime;

    [Header("Combat")]
    public bool IsAggro;
    public bool IsAttacking;

    [Header("Navigation")]
    public Vector3 CurrentDestination;
}
