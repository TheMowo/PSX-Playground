using TMPro;
using UnityEngine;

public class UI_Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        M_GameManager gameManager = M_GameManager.Instance;

        if (gameManager == null)
        {
            timerText.text = "00:00.00";
            return;
        }

        if (!gameManager.GameplayStarted)
        {
            timerText.text = "00:00.00";
            return;
        }

        float time = gameManager.GameplayTime;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int centiseconds = Mathf.FloorToInt((time % 1f) * 100f);

        timerText.text = $"{minutes:00}:{seconds:00}.{centiseconds:00}";
    }
}