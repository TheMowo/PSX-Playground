using TMPro;
using UnityEngine;

public class M_ScoreManager : Singleton<M_ScoreManager>
{
    [SerializeField] private ScoreDatabase database;

    public int CurrentScore { get; private set; }
    public int BonusTimeScore { get; private set; }
    public int TotalScore { get; private set; }

    public int TotalKills { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (database == null)
        {
            Debug.LogError("Score Database has not been assigned.");
        }
    }

    public void RegisterKill()
    {
        TotalKills++;
    }

    public void AddScore(ScoreId id)
    {
        if (!database.TryGet(id, out ScoreDefinition definition))
        {
            Debug.LogWarning($"No ScoreDefinition found for {id}");

            return;
        }

        int amount = definition.BaseValue;

        CurrentScore += amount;

        M_ScoreEvents.OnScoreAdded?.Invoke(new ScoreEventData(definition, amount));
    }

    public void RemoveScore(int amount)
    {
        CurrentScore = Mathf.Max(0, CurrentScore - amount);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        BonusTimeScore = 0;
        TotalScore = 0;
        TotalKills = 0;
    }

    public void FinalizeScores(float gameplayTime)
    {
        BonusTimeScore = CalculateTimeBonus(gameplayTime);
        TotalScore = CurrentScore + BonusTimeScore;
    }

    private int CalculateTimeBonus(float gameplayTime)
    {
        return Mathf.Max(0, 10000 - Mathf.RoundToInt(gameplayTime * 10f));
    }
}