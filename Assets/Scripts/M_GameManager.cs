using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_GameManager : Singleton<M_GameManager>
{
    public GameResults FinalResults { get; private set; }

    #region Unity

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

        // StartGameplay();
    }

    private void Update()
    {
        if (IsTiming)
        {
            gameplayTime += Time.deltaTime;
        }
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

        // StartGameplay();
    }

    #endregion

    #region Gameplay Loop

    public enum GameplayState
    {
        Loading,
        WaitingToStart,
        Playing,
        Winnable,
        Won
    }

    private GameplayState gameState;

    public event Action<GameplayState> OnStateChanged;

    public event Action OnCoreCollected;
    public event Action OnWinnable;
    public event Action OnWin;
    public event Action OnGameplayStarted;

    private readonly HashSet<Core> cores = new();

    private int totalCoreCount;
    private int collectedCoreCount;

    private float gameplayTime;

    public GameplayState State => gameState;

    public bool CanWin => gameState == GameplayState.Winnable;

    public bool GameCompleted => gameState == GameplayState.Won;

    public bool GameplayStarted => gameState == GameplayState.Playing || gameState == GameplayState.Winnable || gameState == GameplayState.Won;

    public float GameplayTime => gameplayTime;

    public int CoreTotal => totalCoreCount;

    public int CurrentCores => collectedCoreCount;

    private bool IsTiming => gameState == GameplayState.Playing && !M_PauseManager.Instance.IsPaused;

    public bool IsGameplayBlocked
    {
        get
        {
            return M_PauseManager.Instance.IsPaused || (TransitionController.Instance != null && TransitionController.Instance.IsTransitioning);
        }
    }

    public void RegisterCore(Core core)
    {
        if (core == null)
            return;

        if (!cores.Add(core))
            return;

        totalCoreCount++;

#if UNITY_EDITOR
        Debug.Log($"Core Registered ({totalCoreCount}) : {core.name}");
#endif
    }

    public void UnregisterCore(Core core)
    {
        if (core == null)
            return;

        if (!cores.Remove(core))
            return;

        totalCoreCount = Mathf.Max(0, totalCoreCount - 1);

        CheckWinCondition();
    }

    public void CoreCollected(Core core)
    {
        if (core == null)
            return;

        collectedCoreCount = Mathf.Min(collectedCoreCount + 1, totalCoreCount);

        Debug.Log($"Core Collected ({collectedCoreCount}/{totalCoreCount})");

        OnCoreCollected?.Invoke();

        M_ScoreManager.Instance.AddScore(ScoreId.CoreCollected);

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        //if (gameState == GameplayState.Winnable ||
        //    gameState == GameplayState.Won)
        //{
        //    return;
        //}

        if (collectedCoreCount >= totalCoreCount)
        {
            SetState(GameplayState.Winnable);

            Debug.Log("All cores collected. Player can now win.");

            OnWinnable?.Invoke();
        }
    }

    private void SetState(GameplayState newState)
    {
        if (gameState == newState)
            return;

        gameState = newState;

        OnStateChanged?.Invoke(gameState);
    }

    public void StartGameplay()
    {
        if (gameState == GameplayState.Playing)
            return;

        gameplayTime = 0f;

        SetState(GameplayState.Playing);

        OnGameplayStarted?.Invoke();

        Debug.Log("Gameplay Started.");
    }

    public void GameplayEnd()
    {
        if (gameState != GameplayState.Winnable)
            return;

        SetState(GameplayState.Won);

        var scoreManager = M_ScoreManager.Instance;

        scoreManager.FinalizeScores(gameplayTime);

        FinalResults = new GameResults(
            FormattedGameplayTime,
            scoreManager.TotalKills,
            scoreManager.CurrentScore,
            scoreManager.BonusTimeScore,
            scoreManager.TotalScore
        );

        Debug.Log("Player Won.");

        OnWin?.Invoke();
    }

    private void ResetValues()
    {
        cores.Clear();

        if (M_PauseManager.Instance != null)
        {
            M_PauseManager.Instance.ClearReasons();
        }

        Time.timeScale = 1f;

        totalCoreCount = 0;
        collectedCoreCount = 0;

        gameplayTime = 0f;

        gameState = GameplayState.WaitingToStart;

        FinalResults = default;
    }

    public string FormattedGameplayTime
    {
        get
        {
            int minutes = Mathf.FloorToInt(gameplayTime / 60f);
            int seconds = Mathf.FloorToInt(gameplayTime % 60f);
            int centiseconds = Mathf.FloorToInt((gameplayTime % 1f) * 100f);

            return $"{minutes:00}:{seconds:00}.{centiseconds:00}";
        }
    }

    #endregion
}