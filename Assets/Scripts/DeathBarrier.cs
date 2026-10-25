using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    [SerializeField] private bool sendToLastCheckpoint = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerPublicRef>(out PlayerPublicRef player))
        {
            if (sendToLastCheckpoint && M_CheckpointManager.Instance != null && M_CheckpointManager.Instance.HasCheckpoint())
            {
                player.GetComponent<PlayerHealth>().TakeDamage(50);
                player.GetComponent<PlayerHealth>().RespawnCheckpoint();
                return;
            }
        }

        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(99999);
        }
    }
}