using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private float baseHealth = 50; // Stored, unchanged
    private float currentHealth; // Active health pool
    private float maxHealth; // In the event that an enemy can gain overhealth, use this. (for capping currentHealth & health gains)

    [SerializeField] private bool isKillable = true;

    [Header("")]
    [SerializeField] private DamagePopup popupPrefab;
    [SerializeField] private Transform headPosition;

    public void Setup()
    {
        currentHealth = baseHealth;
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth < damage && isKillable)
        {
            Death();

            SpawnHitEffect(100);
        }
        else
        {
            currentHealth -= damage;

            SpawnHitEffect(damage);
        }
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }

    private void SpawnHitEffect(float damage)
    {
        Vector3 spawnPos = headPosition.position;

        spawnPos += Camera.main.transform.forward * 4f;

        DamagePopup popup = Instantiate(popupPrefab, headPosition.position, Quaternion.identity);

        popup.Initialize(damage);
    }
}
