using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScoreScreen : MonoBehaviour
{
    [SerializeField] private float delay = 0.25f;

    //private Image rankIcon;
    //private TextMeshProUGUI scoreText;
    [Header("Fields")]
    [SerializeField] private GhostRevealText timeResultText;
    [SerializeField] private GhostRevealText killsResultText;
    [SerializeField] private GhostRevealText pointsText;
    [SerializeField] private GhostRevealText totalText;
    [SerializeField] private GhostRevealText bonusText;

    private Image rankIcon;

    [Header("For Reveal Sequence")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private List<GameObject> gameObjects;

    private void Start()
    {
        M_GameManager.Instance.OnWin += StartRevealSequence;
    }

    private void UpdateWinScreenData(string finalTime, int totalKills, int points, int total, int bonus)
    {
        timeResultText.targetText = finalTime;
        killsResultText.targetText = totalKills.ToString();
        pointsText.targetText = points.ToString();
        totalText.targetText = total.ToString();
        bonusText.targetText = $"+{bonus}";

        StartCoroutine(RevealWinScreen());
    }

    private void StartRevealSequence()
    {
        GameResults results = M_GameManager.Instance.FinalResults;

        UpdateWinScreenData(
            results.Time,
            results.Kills,
            results.Score,
            results.Total,
            results.Bonus
        );
    }

    private IEnumerator RevealWinScreen()
    {
        M_PauseManager.Instance.AddReason(M_PauseManager.PauseReason.WinScreen);
        winScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        foreach(GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    [Button(ButtonSizes.Medium)]
    public void StartRevealButton()
    {
        StartRevealSequence();
    }

    [Button(ButtonSizes.Medium)]
    public void ResetRevealObjectsButton()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (M_PauseManager.Instance.HasReason(M_PauseManager.PauseReason.WinScreen))
        {
            M_PauseManager.Instance.RemoveReason(M_PauseManager.PauseReason.WinScreen);
        }
    }
}
