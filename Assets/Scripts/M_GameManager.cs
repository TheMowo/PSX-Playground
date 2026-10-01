using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_GameManager : MonoBehaviour
{
    public static M_GameManager Instance { get; private set; }

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

    public bool IsGameplayBlocked =>
        IsPaused ||
        (TransitionController.Instance != null &&
         TransitionController.Instance.IsTransitioning);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            M_SceneManager.Instance.LoadScene(1);
    }

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
}