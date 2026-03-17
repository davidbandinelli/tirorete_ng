namespace PBEMFootball.Common.Models;

public enum BenchStaffType
{
    Coach,
    Masseur,
    Scout,
    Tactician
}

public class BenchStaff
{
    public string Name { get; set; } = string.Empty;
    public BenchStaffType Type { get; set; }
    public string? SpecializedTactic { get; set; }

    public int Salary
    {
        get
        {
            return Type == BenchStaffType.Tactician ? 5 : 2;
        }
    }

    public BenchStaff(string name, BenchStaffType type, string? specializedTactic = null)
    {
        Name = name;
        Type = type;
        SpecializedTactic = specializedTactic;
    }
}