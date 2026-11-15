using TMPro;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI activeScoreText;

    [Header("Score Notification")]
    [SerializeField] private GameObject notiPrefab;
    [SerializeField] private Transform rootObject;

    private void Start()
    {
        M_ScoreEvents.OnScoreAdded += UpdateScore;
        M_ScoreEvents.OnScoreAdded += UpdateScoreNotiInfo;
    }

    private void OnDestroy()
    {
        
    }

    public void UpdateScore(ScoreEventData eventData)
    {
        activeScoreText.text = $"{M_ScoreManager.Instance.CurrentScore}";
    }

    public void UpdateScoreNotiInfo(ScoreEventData eventData)
    {
        UI_ScoreNotification notification = Instantiate(notiPrefab, rootObject).GetComponent<UI_ScoreNotification>();
        notification.UpdateText(eventData.Amount, eventData.Definition.DisplayName);
    }
}
