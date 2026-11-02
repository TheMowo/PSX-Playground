using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Health : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider chunkSlider;

    [SerializeField] private float chunkSpeed = 8f;
    [SerializeField] private float chunkDelay = 0.4f;

    private float targetValue;
    private float chunkDelayTimer;

    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthSlider.minValue = 0f;
        healthSlider.maxValue = maxHealth;

        chunkSlider.minValue = 0f;
        chunkSlider.maxValue = maxHealth;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        healthText.text = Mathf.CeilToInt(currentHealth).ToString();

        bool gainedHealth = currentHealth > targetValue;

        targetValue = currentHealth;

        // Front bar always updates immediately
        healthSlider.value = targetValue;

        if (gainedHealth)
        {
            // Healing updates both bars immediately
            chunkSlider.value = targetValue;
            chunkDelayTimer = 0f;
        }
        else
        {
            // Damage delays the chunk bar
            chunkDelayTimer = chunkDelay;
        }
    }

    private void Update()
    {
        if (Mathf.Approximately(chunkSlider.value, targetValue))
            return;

        if (chunkDelayTimer > 0f)
        {
            chunkDelayTimer -= Time.deltaTime;
            return;
        }

        chunkSlider.value = Mathf.Lerp(chunkSlider.value, targetValue, chunkSpeed * Time.deltaTime);

        if (Mathf.Abs(chunkSlider.value - targetValue) < 0.01f)
        {
            chunkSlider.value = targetValue;
        }
    }
}