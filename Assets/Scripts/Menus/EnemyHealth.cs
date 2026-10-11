using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private int baseHealth = 10; // Stored, unchanged
    private int currentHealth; // Active health pool
    private int maxHealth; // In the event that an enemy can gain overhealth, use this. (for capping currentHealth & health gains)

    public void Setup()
    {
        currentHealth = baseHealth;
    }

    public void TakeDamage(int damage)
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
