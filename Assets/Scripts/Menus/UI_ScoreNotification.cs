using System.Collections;
using TMPro;
using UnityEngine;

public class UI_ScoreNotification : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI flairText;

    private const float Lifetime = 2f;
    private const float FadeDuration = 1.5f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void UpdateText(int score, string text)
    {
        scoreText.text = $"+{score}";
        flairText.text = text;
    }

    private void OnEnable()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        StartCoroutine(ExpiryTimer());
    }

    private IEnumerator ExpiryTimer()
    {
        yield return new WaitForSeconds(Lifetime);

        float elapsed = 0f;

        while (elapsed < FadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / FadeDuration);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        Destroy(gameObject);
    }
}