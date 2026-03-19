namespace PBEMFootball.Common.Models;

public class TeamMatchAreaSummary
{
    public Dictionary<string, int> HomeFieldDistribution { get; set; } = new();
    public Dictionary<string, int> HardnessDistribution { get; set; } = new();
    public Dictionary<string, int> GreatPerformanceDistribution { get; set; } = new();
    public int Po { get; set; }
    public int Li { get; set; }
    public int Di { get; set; }
    public int Ce { get; set; }
    public int At { get; set; }
}
