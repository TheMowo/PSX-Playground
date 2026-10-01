using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_SceneManager : MonoBehaviour
{
    public static M_SceneManager Instance { get; private set; }

    [Header("Loading")]
    [SerializeField] private bool dontDestroyOnLoad = true;

    public bool IsLoading { get; private set; }

    public event Action<float> OnLoadProgress;
    public event Action<string> OnSceneLoaded;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }



    #region Scene Loading

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Single));
    }

    public void LoadScene(int buildIndex)
    {
        StartCoroutine(LoadSceneAsync(buildIndex, LoadSceneMode.Single));
    }

    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Additive));
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(UnloadSceneAsync(sceneName));
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion



    #region Coroutines

    private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode)
    {
        if (IsLoading)
            yield break;

        IsLoading = true;

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneName, mode);

        yield return HandleLoading(operation);

        IsLoading = false;
        OnSceneLoaded?.Invoke(sceneName);
    }

    private IEnumerator LoadSceneAsync(int buildIndex, LoadSceneMode mode)
    {
        if (IsLoading)
            yield break;

        IsLoading = true;

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(buildIndex, mode);

        yield return HandleLoading(operation);

        IsLoading = false;
        OnSceneLoaded?.Invoke(SceneManager.GetSceneByBuildIndex(buildIndex).name);
    }

    private IEnumerator UnloadSceneAsync(string sceneName)
    {
        AsyncOperation operation =
            SceneManager.UnloadSceneAsync(sceneName);

        if (operation == null)
            yield break;

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator HandleLoading(AsyncOperation operation)
    {
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            OnLoadProgress?.Invoke(progress);

            yield return null;
        }

        OnLoadProgress?.Invoke(1f);

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    #endregion

    #region Utilities

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}