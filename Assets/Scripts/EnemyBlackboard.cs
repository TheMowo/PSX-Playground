using UnityEngine;

public class EnemyBlackboard : MonoBehaviour // Stores Data
{
    [Header("Target")]
    public Transform CurrentTarget;

    public bool HasSeenTarget;
    public bool CanSeeTarget;

    public Vector3 LastSeenPosition;

    [Header("Combat")]
    public bool IsAggro = false;
    public bool IsAttacking;
    public bool AttackOnCooldown;
    public bool WantsToAttack;

    public bool HasDealtAttackDamage;

    public bool IsAlive = true;

    [Header("Navigation")]
    public Vector3 CurrentDestination;
}
