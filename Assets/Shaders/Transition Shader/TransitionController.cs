using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    [Header("Transition UI")]
    [SerializeField] private Image transitionImage;

    [SerializeField]
    private float duration = 1f;

    [Header("Scene Loading")]
    [SerializeField]
    private bool useAsyncSceneLoading = true;

    private Material transitionMaterial;

    private Coroutine transitionCoroutine;

    static public TransitionController Instance { get; private set; }

    private void Awake()
    {
        transitionImage.gameObject.SetActive(false);
        Instance = this;

        if (transitionImage == null)
        {
            Debug.LogError(
                "TransitionController: " +
                "No Transition Image has been assigned."
            );

            return;
        }

        // Create a unique runtime material instance.
        transitionMaterial =
            new Material(
                transitionImage.material
            );

        transitionMaterial.name =
            "Transition Runtime Material";

        transitionImage.material =
            transitionMaterial;

        // Start fully transparent/open.
        SetCutoff(1f);
    }



    private void OnDestroy()
    {
        if (transitionMaterial != null)
        {
            Destroy(transitionMaterial);
        }
    }


    // =====================================================================
    // PUBLIC API
    // =====================================================================

    /// <summary>
    /// Plays the transition from the visible game
    /// to a fully covered screen.
    /// </summary>
    public void PlayTransition()
    {
        StartTransition(FadeOutInRoutine());
    }

    private IEnumerator FadeOutInRoutine()
    {
        transitionImage.gameObject.SetActive(true);

        // Fade out (screen covered)
        yield return AnimateCutoff(0f, 1f);

        // Wait 3 seconds fully covered
        yield return new WaitForSecondsRealtime(3f);

        // Fade back in (screen revealed)
        yield return AnimateCutoff(1f, 0f);

        transitionCoroutine = null;

        transitionImage.gameObject.SetActive(false);
    }


    /// <summary>
    /// Loads a scene with a transition.
    ///
    /// 1. Cover current scene
    /// 2. Load new scene
    /// 3. Reveal new scene
    /// </summary>
    public void LoadSceneWithTransition(
        string sceneName)
    {
        StartTransition(
            LoadSceneRoutine(sceneName)
        );
    }


    /// <summary>
    /// Same as LoadSceneWithTransition,
    /// but uses a build index.
    /// </summary>
    public void LoadSceneWithTransition(
        int sceneBuildIndex)
    {
        StartTransition(
            LoadSceneRoutine(sceneBuildIndex)
        );
    }


    // =====================================================================
    // TEST
    // =====================================================================

    private void Update()
    {
        // Temporary test.
        //
        // Press T to play the close animation.
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayTransition();
        }
    }


    // =====================================================================
    // TRANSITION CONTROL
    // =====================================================================

    private void StartTransition(
        IEnumerator routine)
    {
        if (transitionMaterial == null)
        {
            Debug.LogError(
                "TransitionController: " +
                "Transition material has not been initialized."
            );

            return;
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );
        }

        transitionCoroutine =
            StartCoroutine(routine);
    }


    private void SetCutoff(float value)
    {
        if (transitionMaterial == null)
        {
            return;
        }

        transitionMaterial.SetFloat(
            "_Cutoff",
            Mathf.Clamp01(value)
        );
    }


    private IEnumerator AnimateCutoff(
        float from,
        float to)
    {
        float elapsed = 0f;

        float transitionDuration =
            Mathf.Max(
                0.0001f,
                duration
            );

        while (elapsed < transitionDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float normalized =
                Mathf.Clamp01(
                    elapsed /
                    transitionDuration
                );

            float eased =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    normalized
                );

            float cutoff =
                Mathf.Lerp(
                    from,
                    to,
                    eased
                );

            SetCutoff(cutoff);

            yield return null;
        }

        SetCutoff(to);

        transitionCoroutine = null;
    }


    // =====================================================================
    // SCENE LOADING
    // =====================================================================

    private IEnumerator LoadSceneRoutine(
        string sceneName)
    {
        // -------------------------------------------------------------
        // COVER CURRENT SCENE
        // -------------------------------------------------------------

        yield return AnimateCutoff(
            1f,
            0f
        );


        // -------------------------------------------------------------
        // LOAD NEW SCENE
        // -------------------------------------------------------------

        if (useAsyncSceneLoading)
        {
            AsyncOperation operation =
                SceneManager.LoadSceneAsync(
                    sceneName
                );

            if (operation == null)
            {
                Debug.LogError(
                    $"TransitionController: " +
                    $"Could not load scene '{sceneName}'."
                );

                yield break;
            }

            operation.allowSceneActivation =
                false;

            while (operation.progress < 0.9f)
            {
                yield return null;
            }

            operation.allowSceneActivation =
                true;

            // Wait for scene activation.
            while (!operation.isDone)
            {
                yield return null;
            }
        }
        else
        {
            SceneManager.LoadScene(
                sceneName
            );

            yield return null;
        }


        // -------------------------------------------------------------
        // REVEAL NEW SCENE
        // -------------------------------------------------------------

        yield return AnimateCutoff(
            0f,
            1f
        );
    }


    private IEnumerator LoadSceneRoutine(
        int sceneBuildIndex)
    {
        // -------------------------------------------------------------
        // COVER CURRENT SCENE
        // -------------------------------------------------------------

        yield return AnimateCutoff(
            1f,
            0f
        );


        // -------------------------------------------------------------
        // LOAD NEW SCENE
        // -------------------------------------------------------------

        if (useAsyncSceneLoading)
        {
            AsyncOperation operation =
                SceneManager.LoadSceneAsync(
                    sceneBuildIndex
                );

            if (operation == null)
            {
                Debug.LogError(
                    "TransitionController: " +
                    $"Could not load scene " +
                    $"with build index " +
                    $"{sceneBuildIndex}."
                );

                yield break;
            }

            operation.allowSceneActivation =
                false;

            while (operation.progress < 0.9f)
            {
                yield return null;
            }

            operation.allowSceneActivation =
                true;

            while (!operation.isDone)
            {
                yield return null;
            }
        }
        else
        {
            SceneManager.LoadScene(
                sceneBuildIndex
            );

            yield return null;
        }


        // -------------------------------------------------------------
        // REVEAL NEW SCENE
        // -------------------------------------------------------------

        yield return AnimateCutoff(
            0f,
            1f
        );
    }


}