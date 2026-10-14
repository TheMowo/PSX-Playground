using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private float baseHealth = 10; // Stored, unchanged
    private float currentHealth; // Active health pool
    private float maxHealth; // In the event that an enemy can gain overhealth, use this. (for capping currentHealth & health gains)

    public void Setup()
    {
        currentHealth = baseHealth;
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth < damage)
        {
            Death();
        }
        else
        {
            currentHealth -= damage;
        }
    }

    private void Death()
    {

    }
}
