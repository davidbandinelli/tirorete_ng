namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;

public class TeamManager
{
    private readonly List<Team> _teams = new();

    public void AddTeam(Team team)
    {
        _teams.Add(team);
    }

    public Team? GetTeam(string name)
    {
        return _teams.FirstOrDefault(t => t.Name == name);
    }

    public List<Team> GetAllTeams()
    {
        return _teams;
    }

    public void PaySalaries(Team team)
    {
        int total = 0;
        foreach (var player in team.Players)
        {
            total += player.Salary;
        }
        foreach (var staff in team.Staff)
        {
            total += staff.Salary;
        }
        team.Money -= total;
    }
}