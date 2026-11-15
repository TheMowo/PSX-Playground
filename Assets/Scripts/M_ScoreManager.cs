using TMPro;
using UnityEngine;

public class M_ScoreManager : Singleton<M_ScoreManager>
{
    [SerializeField] private ScoreDatabase database;
    //[SerializeField] private UI_Score scoreUI;

    public int CurrentScore { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (database == null)
        {
            Debug.LogError("Score Database has not been assigned.");
        }
    }

    public void AddScore(ScoreId id)
    {
        AddScore(id, 1f);
    }

    public void AddScore(ScoreId id, float multiplier)
    {
        if (!database.TryGet(id, out ScoreDefinition definition))
        {
            Debug.LogWarning($"No ScoreDefinition found for {id}");

            return;
        }

        int amount = Mathf.RoundToInt(definition.BaseValue * multiplier);

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
    }
}