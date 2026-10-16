using UnityEngine;

public class FilthAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    [Header("Collision")]
    [SerializeField] private LayerMask damageableLayers;

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

        bool validLayer =
            (damageableLayers.value & (1 << other.gameObject.layer)) != 0;

        if (!validLayer)
            return;

        if (!other.TryGetComponent(out IDamageable damageable))
            return;

        damageable.TakeDamage(damage);

        blackboard.HasDealtAttackDamage = true;

        Debug.Log($"{name} dealt {damage} damage to {other.name}");
    }
}