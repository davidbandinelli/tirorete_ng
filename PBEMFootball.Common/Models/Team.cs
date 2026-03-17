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
    public int SpecialPoints { get; set; }
    public Stadium Stadium { get; set; } = new();

    public Team(string name, string managerName)
    {
        Name = name;
        ManagerName = managerName;
        Money = 0;
        TrainingPoints = 0;
        GreatPerformancePoints = 30;
        SpecialPoints = 0;
    }

    /// <summary>
    /// Converte 1 PS in 1 PA (Punto Allenamento)
    /// </summary>
    public bool ConvertSpecialPointsToTraining(int amount)
    {
        if (SpecialPoints < amount) return false;
        SpecialPoints -= amount;
        TrainingPoints += amount;
        return true;
    }

    /// <summary>
    /// Converte 1 PS in 1 PGP (Punto Grande Prestazione)
    /// </summary>
    public bool ConvertSpecialPointsToGreatPerformance(int amount)
    {
        if (SpecialPoints < amount) return false;
        SpecialPoints -= amount;
        GreatPerformancePoints += amount;
        return true;
    }

    /// <summary>
    /// Converte 1 PS in 20 M (Milioni)
    /// </summary>
    public bool ConvertSpecialPointsToMoney(int amount)
    {
        if (SpecialPoints < amount) return false;
        SpecialPoints -= amount;
        Money += amount * 20;
        return true;
    }
}
