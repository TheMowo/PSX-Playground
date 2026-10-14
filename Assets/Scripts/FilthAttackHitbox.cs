using UnityEngine;

public class FilthAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private EnemyBlackboard blackboard;

    private void Awake()
    {
        blackboard = GetComponentInParent<EnemyBlackboard>();

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (blackboard == null)
            return;

        if (blackboard.HasDealtAttackDamage)
            return;

        if (!other.TryGetComponent(out IDamageable damageable))
            return;

        damageable.TakeDamage(damage);

        blackboard.HasDealtAttackDamage = true;

        Debug.Log($"{name} dealt {damage} damage to {other.name}");
    }
}