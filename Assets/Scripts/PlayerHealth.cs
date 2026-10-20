using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float startingHealth = 100f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    public bool IsDead { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = Mathf.Clamp(startingHealth, 0f, maxHealth);
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead)
            return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0f)
        {
            Death();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void SetHealth(float value)
    {
        if (IsDead)
            return;

        CurrentHealth = Mathf.Clamp(value, 0f, MaxHealth);

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0f)
        {
            Death();
        }
    }

    private void Death()
    {
        if (IsDead)
            return;

        IsDead = true;

        OnDeath?.Invoke();

        Debug.Log("Player died.");
    }
}