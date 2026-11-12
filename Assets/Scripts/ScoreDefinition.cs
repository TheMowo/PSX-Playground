using UnityEngine;

[CreateAssetMenu(fileName = "ScoreDefinition", menuName = "Score/Score Definition")]
public class ScoreDefinition : ScriptableObject
{
    [field: SerializeField] public ScoreId Id { get; private set; }

    [field: SerializeField] public int BaseValue { get; private set; } = 100;

    [field: SerializeField] public string DisplayName { get; private set; } = "SCORE";

    [field: SerializeField] public Color PopupColor { get; private set; } = Color.white;
}