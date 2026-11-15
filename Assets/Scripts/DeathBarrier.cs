using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    [SerializeField] private bool sendToLastCheckpoint = false;
    [SerializeField] private DamageType type = DamageType.Lava;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerPublicRef>(out PlayerPublicRef player))
        {
            if (sendToLastCheckpoint && M_CheckpointManager.Instance != null && M_CheckpointManager.Instance.HasCheckpoint())
            {
                DamageType damageType = type;
                DamageInfo damageInfo = new DamageInfo(25, damageType, gameObject);

                player.GetComponent<PlayerHealth>().TakeDamage(damageInfo);
                player.GetComponent<PlayerRespawn>().RespawnAtCheckpoint();
                return;
            }
        }

        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            DamageType damageType = type;

            DamageInfo damageInfo = new DamageInfo(999999, damageType, gameObject);

            damageable.TakeDamage(damageInfo);
        }
    }
}