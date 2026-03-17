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

    /// <summary>
    /// Verifica se è possibile aggiungere un membro dello staff secondo le regole.
    /// Regola: max 1 Allenatore, 1 Massaggiatore, 1 Tattico. Più Scout permessi.
    /// </summary>
    public bool CanAddStaff(BenchStaffType staffType)
    {
        if (staffType == BenchStaffType.Scout)
            return true; // Scout multipli permessi

        // Coach, Masseur, Tactician: solo 1 per tipo
        return !Staff.Any(s => s.Type == staffType);
    }

    /// <summary>
    /// Aggiunge uno staff member se le regole lo permettono
    /// </summary>
    public bool TryAddStaff(BenchStaff staffMember)
    {
        if (!CanAddStaff(staffMember.Type))
            return false;

        Staff.Add(staffMember);
        return true;
    }

    /// <summary>
    /// Conta il numero di staff per tipo
    /// </summary>
    public int GetStaffCount(BenchStaffType staffType)
    {
        return Staff.Count(s => s.Type == staffType);
    }
}
