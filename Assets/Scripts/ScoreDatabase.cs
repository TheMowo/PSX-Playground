using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreDatabase", menuName = "Score/Score Database")]
public class ScoreDatabase : ScriptableObject
{
    [SerializeField] private List<ScoreDefinition> definitions = new();

    private Dictionary<ScoreId, ScoreDefinition> lookup;

    private void EnsureInitialized()
    {
        if (lookup != null)
        {
            return;
        }

        lookup = new();

        foreach (ScoreDefinition definition in definitions)
        {
            if (definition == null)
            {
                continue;
            }

            if (lookup.ContainsKey(definition.Id))
            {
                Debug.LogError($"Duplicate ScoreId detected: {definition.Id}");

                continue;
            }

            lookup.Add(definition.Id, definition);
        }
    }

    public bool TryGet(ScoreId id, out ScoreDefinition definition)
    {
        EnsureInitialized();

        return lookup.TryGetValue(id, out definition);
    }

    public ScoreDefinition Get(ScoreId id)
    {
        EnsureInitialized();

        if (lookup.TryGetValue(id, out ScoreDefinition definition))
        {
            return definition;
        }

        Debug.LogError($"No ScoreDefinition found for {id}");

        return null;
    }
}