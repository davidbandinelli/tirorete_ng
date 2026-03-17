namespace PBEMFootball.Common.Models;

public class Season
{
    public int Year { get; set; }
    public List<Competition> Competitions { get; set; } = new();
    public List<GameSession> Sessions { get; set; } = new();
    public int CurrentSession { get; set; }
    public bool IsCompleted { get; set; }

    public Season(int year)
    {
        Year = year;
        CurrentSession = 1;
    }

    public Competition? GetChampionship(Division division)
    {
        return Competitions.FirstOrDefault(c => 
            c.Type == CompetitionType.Championship && c.Division == division);
    }

    public Competition? GetCup(CompetitionType cupType)
    {
        return Competitions.FirstOrDefault(c => c.Type == cupType);
    }
}

