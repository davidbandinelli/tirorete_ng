namespace PBEMFootball.Common.Models;

public class Competition
{
    public string Name { get; set; } = string.Empty;
    public CompetitionType Type { get; set; }
    public Division? Division { get; set; }
    public List<Team> Teams { get; set; } = new();
    public List<Match> Matches { get; set; } = new();
    public List<Standing> Standings { get; set; } = new();
    public int CurrentRound { get; set; }
    public bool IsCompleted { get; set; }

    public Competition(string name, CompetitionType type, Division? division = null)
    {
        Name = name;
        Type = type;
        Division = division;
    }
}

public class Standing
{
    public Team Team { get; set; }
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int Points { get; set; }

    public int GoalDifference => GoalsFor - GoalsAgainst;

    public Standing(Team team)
    {
        Team = team;
    }

    public void AddMatch(int goalsFor, int goalsAgainst)
    {
        Played++;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;

        if (goalsFor > goalsAgainst)
        {
            Won++;
            Points += 3;
        }
        else if (goalsFor == goalsAgainst)
        {
            Drawn++;
            Points += 1;
        }
        else
        {
            Lost++;
        }
    }
}
