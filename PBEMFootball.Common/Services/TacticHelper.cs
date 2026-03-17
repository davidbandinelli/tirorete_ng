namespace PBEMFootball.Common.Services;

using PBEMFootball.Common.Models;

public static class TacticHelper
{
    public static string GenerateTacticString(Formation formation, FormationTactics tactics)
    {
        int defenders = formation.Defenders.Count;
        int midfielders = formation.Midfielders.Count;
        int attackers = formation.Attackers.Count;

        string formationStr = $"{defenders}-{midfielders}-{attackers}";

        if (formation.Libero != null)
            formationStr += " Li";

        if (tactics.UseOffsideTrap)
            formationStr += " TFG";

        return formationStr;
    }

    public static bool TacticMatches(string tacticianTactic, Formation formation, FormationTactics tactics)
    {
        if (string.IsNullOrWhiteSpace(tacticianTactic))
            return false;
        
        string currentTactic = GenerateTacticString(formation, tactics);
        
        return string.Equals(currentTactic, tacticianTactic.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
