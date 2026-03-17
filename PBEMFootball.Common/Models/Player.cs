namespace PBEMFootball.Common.Models;

public class Player
{
    public string Name { get; set; } = string.Empty;
    public PlayerPosition Position { get; set; }
    public int Ability { get; set; }
    public int Form { get; set; }
    public int StartingAbility { get; set; }
    public PlayerAge Age { get; set; }
    public PlayerSide Side { get; set; }

    public int DisciplinePoints { get; set; }
    public int ExperiencePoints { get; set; }
    public int MatchesPlayedThisSession { get; set; }
    public int OutOfPositionMatches { get; set; }
    public PlayerPosition? SecondaryPosition { get; set; }

    public int Salary
    {
        get
        {
            if (Age == PlayerAge.Primavera)
                return 0;
            if (Age == PlayerAge.Juniores)
                return 1;
            return (StartingAbility / 5) + 4;
        }
    }

    public bool IsInjured => Form <= -3;

    public bool CanPlayInPosition(PlayerPosition position)
    {
        if (position == Position)
            return true;
        if (SecondaryPosition.HasValue && position == SecondaryPosition.Value)
            return true;
        return false;
    }

    public Player(string name, PlayerPosition position, int ability, PlayerAge age)
    {
        Name = name;
        Position = position;
        Ability = ability;
        StartingAbility = ability;
        Age = age;
    }
}