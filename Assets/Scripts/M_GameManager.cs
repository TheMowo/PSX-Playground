using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class M_GameManager : Singleton<M_GameManager>
{
    protected override void Awake()
    {
        base.Awake();

        ResetValues();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            M_SceneManager.Instance.LoadScene(1);
            return;
        }

        VerifyCores();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetValues();
        VerifyCores();
    }

    #region Pause Related

    public static event Action<bool> PauseStateChanged;

    public enum PauseReason
    {
        PauseMenu,
        Dialogue,
        Inventory,
        Map,
        Cutscene
    }

    private readonly HashSet<PauseReason> pauseReasons = new();

    private bool isPaused;

    public bool IsPaused => isPaused;

    public bool IsGameplayBlocked => IsPaused || (TransitionController.Instance != null && TransitionController.Instance.IsTransitioning);

    public void AddPauseReason(PauseReason reason)
    {
        if (!pauseReasons.Add(reason))
            return;

        RefreshPauseState();
    }

    public void RemovePauseReason(PauseReason reason)
    {
        if (!pauseReasons.Remove(reason))
            return;

        RefreshPauseState();
    }

    public bool HasPauseReason(PauseReason reason)
    {
        return pauseReasons.Contains(reason);
    }

    public void ClearPauseReasons()
    {
        if (pauseReasons.Count == 0)
            return;

        pauseReasons.Clear();
        RefreshPauseState();
    }

    private void RefreshPauseState()
    {
        bool shouldPause = pauseReasons.Count > 0;

        if (shouldPause == isPaused)
            return;

        isPaused = shouldPause;

        Time.timeScale = isPaused ? 0f : 1f;

        PauseStateChanged?.Invoke(isPaused);
    }

    #endregion

    #region Gameplay Loop

    public event Action OnCoreCollected;
    public event Action OnWinnable;
    public event Action OnWin;

    private bool canWin;
    private int coreTotal;
    private int currentCores;

    public bool CanWin => canWin;
    public int CurrentCores => currentCores;
    public int CoreTotal => coreTotal;

    private void ResetValues()
    {
        canWin = false;
        coreTotal = 0;
        currentCores = 0;
    }

    private void VerifyCores()
    {
        Core[] coreList = FindObjectsByType<Core>(
            FindObjectsSortMode.None);

        coreTotal = coreList.Length;

        Debug.Log($"Found {coreTotal} cores in scene.");
    }

    public void CoreCollected()
    {
        currentCores++;

        Debug.Log($"Core Collected ({currentCores}/{coreTotal})");

        OnCoreCollected?.Invoke();

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (canWin)
            return;

        if (currentCores >= coreTotal)
        {
            canWin = true;

            Debug.Log("All cores collected. Player can now win.");

            OnWinnable?.Invoke();
        }
    }

    public void Win()
    {
        if (!canWin)
            return;

        Debug.Log("Player Won.");

        OnWin?.Invoke();
    }

    #endregion

    #region Score Related



    #endregion
}