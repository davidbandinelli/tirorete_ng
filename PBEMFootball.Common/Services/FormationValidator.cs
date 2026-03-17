namespace PBEMFootball.Common.Services;

using PBEMFootball.Common.Models;

public class FormationValidator
{
    public bool IsValid(Formation formation)
    {
        if (formation.Goalkeeper == null)
            return false;
        if (formation.Defenders.Count < 2)
            return false;
        if (formation.Midfielders.Count < 2)
            return false;
        if (formation.Attackers.Count < 2)
            return false;

        int total = 1 + (formation.Libero != null ? 1 : 0) +
                    formation.Defenders.Count +
                    formation.Midfielders.Count +
                    formation.Attackers.Count;
        return total == 11;
    }
}