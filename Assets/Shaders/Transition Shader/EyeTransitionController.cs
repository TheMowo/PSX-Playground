using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EyeTransitionController : MonoBehaviour
{
    [SerializeField] private GameObject transitionHolder;
    private Material material;
    [SerializeField] private float transitionDuration = 0.1f;

    private Coroutine currentTransition;

    private static readonly int CutoffID = Shader.PropertyToID("_Cutoff");

    private void Awake()
    {
        material = transitionHolder.GetComponent<Image>().material;
    }

    private void OnDisable()
    {
        material.SetFloat(CutoffID, 0);
    }

    private void OnDestroy()
    {
        material.SetFloat(CutoffID, 0);
    }

    public void StartAnimation()
    {
        StartTransition(0f, 1f);
    }

    public void ExitAnimation()
    {
        StartTransition(1f, 0f);
    }

    private void StartTransition(float startValue, float endValue)
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        currentTransition = StartCoroutine(
            AnimateCutoff(startValue, endValue)
        );
    }

    private IEnumerator AnimateCutoff(float startValue, float endValue)
    {
        float elapsed = 0f;

        material.SetFloat(CutoffID, startValue);

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / transitionDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            material.SetFloat(
                CutoffID,
                Mathf.Lerp(startValue, endValue, t)
            );

            yield return null;
        }

        material.SetFloat(CutoffID, endValue);

        currentTransition = null;
    }
}