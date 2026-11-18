using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class M_PauseManager : Singleton<M_PauseManager>
{
    public enum PauseReason
    {
        PauseMenu,
        Dialogue,
        Inventory,
        Map,
        Cutscene,
        WinScreen
    }

    public static event Action<bool> PauseStateChanged;

    private readonly HashSet<PauseReason> pauseReasons = new();

    private bool isPaused;

    public bool IsPaused => isPaused;

    public int ActiveReasonCount => pauseReasons.Count;

    public bool HasReason(PauseReason reason)
    {
        return pauseReasons.Contains(reason);
    }

    public void AddReason(PauseReason reason)
    {
        if (!pauseReasons.Add(reason))
            return;

        RefreshPauseState();
    }

    public void RemoveReason(PauseReason reason)
    {
        if (!pauseReasons.Remove(reason))
            return;

        RefreshPauseState();
    }

    public void ToggleReason(PauseReason reason)
    {
        if (pauseReasons.Contains(reason))
            RemoveReason(reason);
        else
            AddReason(reason);
    }

    public void ClearReasons()
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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
        }
    }
}