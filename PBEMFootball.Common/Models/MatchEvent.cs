namespace PBEMFootball.Common.Models;

public enum MatchEventType
{
    Goal,
    Shot,
    ShotMissed,
    ShotSavedByGoalkeeper,
    ShotBlockedByLibero,
    Penalty,
    PenaltyMissed,
    OwnGoal,
    YellowCard,
    RedCard,
    Injury,
    AreaReduction,
    Save,
    MissedShot,
    HitPost
}

public class MatchEvent
{
    public MatchEventType Type { get; set; }
    public int Minute { get; set; }
    public Player? Player { get; set; }
    public bool IsHomeTeam { get; set; }
    public string Description { get; set; } = string.Empty;
}
