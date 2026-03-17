namespace PBEMFootball.Common.Models;

public class FormationTactics
{
    public int HomeFieldAdvantagePoints { get; set; }
    public Dictionary<string, int> HomeFieldAdvantageDistribution { get; set; } = new();
    public int HardnessTotal { get; set; }
    public Dictionary<string, int> HardnessDistribution { get; set; } = new();
    public int GreatPerformancePointsTotal { get; set; }
    public Dictionary<string, int> GreatPerformanceDistribution { get; set; } = new();
    public bool UseCatenaccio { get; set; }
    public int CatenaccioPoints { get; set; }
    public Dictionary<string, int> CatenaccioDistribution { get; set; } = new();
    public bool UseOffsideTrap { get; set; }
    public int TacticianBonusPoints { get; set; }
    public Dictionary<string, int> TacticianDistribution { get; set; } = new();
}
