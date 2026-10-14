using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        Debug.Log($"Player hit for {amount} DMG. Remaining Health: {currentHealth}");

        if (currentHealth <= 0)
            Death();
    }

    private void Death()
    {
        Debug.Log("Player Died");
    }
}