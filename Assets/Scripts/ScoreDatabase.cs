#if UNITY_EDITOR
using System;
#endif

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreDatabase", menuName = "Score/Score Database")]
public class ScoreDatabase : ScriptableObject
{
    [SerializeField] private List<ScoreDefinition> definitions = new();

    private Dictionary<ScoreId, ScoreDefinition> lookup;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Debug.isDebugBuild)
            return;

        ValidateDefinitions();
    }
#endif

    private void EnsureInitialized()
    {
        if (lookup != null)
            return;

        lookup = new();

        foreach (ScoreDefinition definition in definitions)
        {
            if (definition == null)
                continue;

            if (lookup.ContainsKey(definition.Id))
            {
                Debug.LogError($"Duplicate ScoreId detected: {definition.Id}", this);

                continue;
            }

            lookup.Add(definition.Id, definition);
        }
    }

#if UNITY_EDITOR
    private void ValidateDefinitions()
    {
        HashSet<ScoreId> assignedIds = new();

        foreach (ScoreDefinition definition in definitions)
        {
            if (definition == null)
            {
                Debug.LogWarning("ScoreDatabase contains a null ScoreDefinition reference.", this);

                continue;
            }

            if (!assignedIds.Add(definition.Id))
            {
                Debug.LogWarning($"ScoreDatabase contains multiple definitions for ScoreId.{definition.Id}", this);
            }
        }

        foreach (ScoreId id in (ScoreId[])Enum.GetValues(typeof(ScoreId)))
        {
            if (assignedIds.Contains(id))
                continue;

            Debug.LogWarning($"ScoreDatabase is missing a ScoreDefinition for ScoreId.{id}", this);
        }
    }
#endif

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