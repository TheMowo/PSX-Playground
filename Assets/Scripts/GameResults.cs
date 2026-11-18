public readonly struct GameResults
{
    public readonly string Time;
    public readonly int Kills;
    public readonly int Score;
    public readonly int Bonus;
    public readonly int Total;

    public GameResults(
        string time,
        int kills,
        int score,
        int bonus,
        int total)
    {
        Time = time;
        Kills = kills;
        Score = score;
        Bonus = bonus;
        Total = total;
    }
}