using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIFader : MonoBehaviour
{
    public static UIFader Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 3f;

    // Curve goes from 0 --> 1 over time
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("UIFader: CanvasGroup not assigned!");
            return;
        }

        canvasGroup.alpha = 1f; // start black
        //StartCoroutine(FadeIn());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }

    // Fade Coroutines
    public IEnumerator FadeIn()
    {
        Debug.Log("FadeIn running");
        yield return new WaitForSeconds(0.5f);
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeDuration;
            float curveValue = fadeCurve.Evaluate(normalized); // 0 --> 1 shaped by curve
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, curveValue);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Debug.Log("FadeIn completed");
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeDuration;
            float curveValue = fadeCurve.Evaluate(normalized);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, curveValue);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public IEnumerator DebugFadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeDuration;
            float curveValue = fadeCurve.Evaluate(normalized);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, curveValue);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        new WaitForSeconds(3f);
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return FadeOut();
        SceneManager.LoadScene(sceneName);
        // FadeIn runs automatically after load
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(FadeIn());
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(FadeOut());
        }
    }
}
