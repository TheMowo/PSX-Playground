using TMPro;
using UnityEngine;

public class M_ObjectiveManager : Singleton<M_ObjectiveManager>
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float remainingTime;
    private bool timerRunning;
    private Transform currentDestination;

    private void Update()
    {
        if (timerRunning)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                EndTimer();
            }
            UpdateTimerUI();
        }
    }

    public void StartTimer(float minutes, float seconds)
    {
        remainingTime = (minutes * 60f) + seconds;
        timerRunning = true;
        UpdateTimerUI();
        Debug.Log($"Objective timer started: {minutes}m {seconds}s");
    }

    public void EndTimer()
    {
        timerRunning = false;
        remainingTime = 0f;
        UpdateTimerUI();
        Debug.Log("Objective timer ended!");
        // TODO: trigger failure state if delivery not completed
    }

    public void SetDestination(Transform destination)
    {
        currentDestination = destination;
        Debug.Log($"Objective destination set: {destination.name}");
        // TODO: update minimap/marker UI to point to destination
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
