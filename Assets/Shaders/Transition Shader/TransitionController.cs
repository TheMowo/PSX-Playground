using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    [Header("Transition UI")]
    [SerializeField] private Image transitionImage;
    [SerializeField] private float duration = 1f;

    [Header("Scene Loading")]
    [SerializeField] private bool useAsyncSceneLoading = true;

    private Material transitionMaterial;
    private Coroutine transitionCoroutine;

    private bool isTransitioning;
    private bool isLoadingScene;

    public bool IsTransitioning => isTransitioning;
    public bool IsLoadingScene => isLoadingScene;

    public static TransitionController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transitionImage == null)
        {
            Debug.LogError("TransitionController: No Transition Image has been assigned.");
            return;
        }

        transitionImage.gameObject.SetActive(false);
        transitionImage.raycastTarget = true;

        transitionMaterial = new Material(transitionImage.material);
        transitionMaterial.name = "Transition Runtime Material";

        transitionImage.material = transitionMaterial;

        // Start fully open.
        SetCutoff(1f);
    }

    private void OnDestroy()
    {
        if (transitionMaterial != null)
            Destroy(transitionMaterial);
    }

    // =====================================================================
    // PUBLIC API
    // =====================================================================

    public void PlayTransition()
    {
        if (isTransitioning)
            return;

        StartTransition(FadeOutRoutine());
    }

    public void LoadSceneWithTransition(string sceneName)
    {
        if (isTransitioning || isLoadingScene)
            return;

        isLoadingScene = true;
        StartTransition(LoadSceneRoutine(sceneName));
    }

    public void LoadSceneWithTransition(int sceneBuildIndex)
    {
        if (isTransitioning || isLoadingScene)
            return;

        isLoadingScene = true;
        StartTransition(LoadSceneRoutine(sceneBuildIndex));
    }

    // =====================================================================
    // TEST
    // =====================================================================

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //    PlayTransition();
    }

    // =====================================================================
    // TRANSITION CONTROL
    // =====================================================================

    private void StartTransition(IEnumerator routine)
    {
        if (transitionMaterial == null)
        {
            Debug.LogError("TransitionController: Transition material has not been initialized.");
            return;
        }

        if (isTransitioning)
            return;

        transitionCoroutine = StartCoroutine(TransitionWrapper(routine));
    }

    private IEnumerator TransitionWrapper(IEnumerator routine)
    {
        isTransitioning = true;

        transitionImage.gameObject.SetActive(true);

        yield return routine;

        isTransitioning = false;
        transitionCoroutine = null;
    }

    private void SetCutoff(float value)
    {
        if (transitionMaterial == null)
            return;

        transitionMaterial.SetFloat("_Cutoff", Mathf.Clamp01(value));
    }

    private IEnumerator AnimateCutoff(float from, float to)
    {
        float elapsed = 0f;
        float transitionDuration = Mathf.Max(0.0001f, duration);

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float normalized = Mathf.Clamp01(elapsed / transitionDuration);
            float eased = Mathf.SmoothStep(0f, 1f, normalized);
            float cutoff = Mathf.Lerp(from, to, eased);

            SetCutoff(cutoff);

            yield return null;
        }

        SetCutoff(to);
    }

    private IEnumerator FadeOutRoutine()
    {
        yield return AnimateCutoff(0f, 1f);
    }

    // =====================================================================
    // SCENE LOADING
    // =====================================================================

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Cover current scene.
        yield return AnimateCutoff(0f, 1f);

        // Load new scene.
        if (useAsyncSceneLoading)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            if (operation == null)
            {
                Debug.LogError($"TransitionController: Could not load scene '{sceneName}'.");
                isLoadingScene = false;
                yield break;
            }

            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
                yield return null;

            operation.allowSceneActivation = true;

            while (!operation.isDone)
                yield return null;
        }
        else
        {
            M_SceneManager.Instance.LoadScene(sceneName);
            yield return null;
        }

        // Reveal new scene.
        yield return AnimateCutoff(1f, 0f);

        transitionImage.gameObject.SetActive(false);
        isLoadingScene = false;
    }

    private IEnumerator LoadSceneRoutine(int sceneBuildIndex)
    {
        // Cover current scene.
        yield return AnimateCutoff(1f, 0f);

        // Load new scene.
        if (useAsyncSceneLoading)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex);

            if (operation == null)
            {
                Debug.LogError($"TransitionController: Could not load scene with build index {sceneBuildIndex}.");
                isLoadingScene = false;
                yield break;
            }

            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
                yield return null;

            operation.allowSceneActivation = true;

            while (!operation.isDone)
                yield return null;
        }
        else
        {
            M_SceneManager.Instance.LoadScene(sceneBuildIndex);
            yield return null;
        }

        // Reveal new scene.
        yield return AnimateCutoff(0f, 1f);

        transitionImage.gameObject.SetActive(false);
        isLoadingScene = false;
    }
}