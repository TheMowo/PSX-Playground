using TMPro;
using UnityEngine;
using System.Collections;

public class UI_InfoBox : Singleton<UI_InfoBox>
{
    [Header("References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private GameObject infoBox;

    [Header("Animation")]
    [SerializeField] private float popupDuration = 0.35f;
    [SerializeField] private float overshootScale = 1.1f;

    private RectTransform rectTransform;
    private Coroutine popupRoutine;

    protected override void Awake()
    {
        base.Awake();

        rectTransform = infoBox.GetComponent<RectTransform>();

        infoBox.SetActive(false);
    }

    public void Show(string title, string body)
    {
        titleText.text = title;
        bodyText.text = body;

        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        popupRoutine = StartCoroutine(PopupAnimation());
    }

    public void Hide()
    {
        infoBox.SetActive(false);
    }

    private IEnumerator PopupAnimation()
    {
        infoBox.SetActive(true);

        rectTransform.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < popupDuration)
        {
            timer += Time.deltaTime;

            float t = timer / popupDuration;

            float scale;

            if (t < 0.7f)
            {
                scale = Mathf.Lerp(
                    0f,
                    overshootScale,
                    t / 0.7f
                );
            }
            else
            {
                scale = Mathf.Lerp(
                    overshootScale,
                    1f,
                    (t - 0.7f) / 0.3f
                );
            }

            rectTransform.localScale = Vector3.one * scale;

            yield return null;
        }

        rectTransform.localScale = Vector3.one;
    }
}
