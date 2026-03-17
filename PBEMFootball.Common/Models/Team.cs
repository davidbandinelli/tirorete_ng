namespace PBEMFootball.Common.Models;

public class Team
{
    public string Name { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public List<Player> Players { get; set; } = new();
    public List<BenchStaff> Staff { get; set; } = new();
    public int Money { get; set; }
    public int TrainingPoints { get; set; }
    public int GreatPerformancePoints { get; set; }
    public Stadium Stadium { get; set; } = new();

    public Team(string name, string managerName)
    {
        Name = name;
        ManagerName = managerName;
        Money = 0;
        TrainingPoints = 0;
        GreatPerformancePoints = 30;
    }
}