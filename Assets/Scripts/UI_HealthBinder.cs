using UnityEngine;

public class UI_HealthBinder : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private UI_Health healthUI;

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += healthUI.SetHealth;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= healthUI.SetHealth;
    }
}