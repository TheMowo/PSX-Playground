using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    [SerializeField] private bool sendToLastCheckpoint = false;

    private void OnTriggerEnter(Collider other)
    {
        if (sendToLastCheckpoint)
        {
            if (other.TryGetComponent<PlayerPublicRef>(out PlayerPublicRef player))
            {
                // player.ResetToLastCheckpoint();
                // need checkpoint system!
            }
        }

        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            if (other.TryGetComponent<PlayerPublicRef>(out PlayerPublicRef player))
                damageable.TakeDamage(50);

            else
                damageable.TakeDamage(99999);
        }
    }
}
