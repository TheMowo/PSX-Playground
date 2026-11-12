public readonly struct ScoreEventData
{
    public readonly ScoreDefinition Definition;
    public readonly int Amount;

    public ScoreEventData(ScoreDefinition definition, int amount)
    {
        Definition = definition;
        Amount = amount;
    }
}