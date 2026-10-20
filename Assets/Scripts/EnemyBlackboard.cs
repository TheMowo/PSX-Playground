using UnityEngine;

public class EnemyBlackboard : MonoBehaviour
{
    [Header("Target")]
    public Transform CurrentTarget;

    public bool HasSeenTarget;
    public bool CanSeeTarget;

    [Header("Combat")]
    public bool IsAggro;
    public bool IsAttacking;
    public bool AttackOnCooldown;
    public bool WantsToAttack;
    public bool HasDealtAttackDamage;

    public bool IsAlive = true;

    [Header("Navigation")]
    public Vector3 CurrentDestination;
}