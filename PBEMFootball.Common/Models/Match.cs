namespace PBEMFootball.Common.Models;

public enum WeatherCondition
{
    Sunny,
    Covered,
    Rainy,
    HeavyRain,
    Suspended
}

public class Match
{
    public Team HomeTeam { get; set; }
    public Team AwayTeam { get; set; }
    public Formation? HomeFormation { get; set; }
    public Formation? AwayFormation { get; set; }
    public FormationTactics? HomeTactics { get; set; }
    public FormationTactics? AwayTactics { get; set; }
    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }
    public int HomeShots { get; set; }
    public int AwayShots { get; set; }
    public bool IsPlayed { get; set; }
    public WeatherCondition Weather { get; set; }
    public List<MatchEvent> Events { get; set; } = new();
    public bool IsHomeMatch => true;

    public Match(Team home, Team away)
    {
        HomeTeam = home;
        AwayTeam = away;
    }
}