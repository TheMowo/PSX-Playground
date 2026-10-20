using UnityEngine;

public class HealthDebugTester : MonoBehaviour
{
    [SerializeField] private UI_Health healthUI;

    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;

    private void Start()
    {
        healthUI.SetHealth(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            currentHealth = Mathf.Clamp(currentHealth - 10f, 0f, maxHealth);
            healthUI.SetHealth(currentHealth, maxHealth);
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            currentHealth = Mathf.Clamp(currentHealth + 10f, 0f, maxHealth);
            healthUI.SetHealth(currentHealth, maxHealth);
        }
    }
}