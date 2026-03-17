namespace PBEMFootball.Common.Services;

using PBEMFootball.Common.Models;

public class TacticsValidator
{
    public bool ValidateTactics(Formation formation, FormationTactics tactics, out string error)
    {
        error = string.Empty;

        if (tactics.UseOffsideTrap && formation.Libero != null)
        {
            error = "La Trappola del Fuori Gioco può essere attivata solo se NON è presente un libero.";
            return false;
        }

        if (tactics.UseCatenaccio)
        {
            int catenaccioTotal = tactics.CatenaccioDistribution.Values.Sum();
            if (catenaccioTotal != 7)
            {
                error = $"Il Catenaccio richiede esattamente 7 punti distribuiti su Li/Di/Ce. Trovati: {catenaccioTotal}";
                return false;
            }

            if (tactics.CatenaccioDistribution.ContainsKey("Po") || 
                tactics.CatenaccioDistribution.ContainsKey("At"))
            {
                error = "Il Catenaccio può essere usato solo su Li, Di e Ce.";
                return false;
            }
        }

        if (tactics.HomeFieldAdvantagePoints > 0)
        {
            int fcTotal = tactics.HomeFieldAdvantageDistribution.Values.Sum();
            if (fcTotal != tactics.HomeFieldAdvantagePoints)
            {
                error = $"Il Fattore Campo richiede che tutti i {tactics.HomeFieldAdvantagePoints} punti siano distribuiti su Di/Ce/At. Distribuiti: {fcTotal}";
                return false;
            }

            if (tactics.HomeFieldAdvantageDistribution.ContainsKey("Po") || 
                tactics.HomeFieldAdvantageDistribution.ContainsKey("Li"))
            {
                error = "Il Fattore Campo può essere usato solo su Di, Ce e At.";
                return false;
            }
        }

        var poExtra = (tactics.HardnessDistribution.GetValueOrDefault("Po", 0) +
                       tactics.GreatPerformanceDistribution.GetValueOrDefault("Po", 0));
        if (poExtra > 5)
        {
            error = $"Sul Portiere non possono essere usati più di 5 punti extra totali. Trovati: {poExtra}";
            return false;
        }

        var liExtra = (tactics.HardnessDistribution.GetValueOrDefault("Li", 0) +
                       tactics.GreatPerformanceDistribution.GetValueOrDefault("Li", 0) +
                       tactics.CatenaccioDistribution.GetValueOrDefault("Li", 0) +
                       tactics.TacticianDistribution.GetValueOrDefault("Li", 0));
        if (liExtra > 5)
        {
            error = $"Sul Libero non possono essere usati più di 5 punti extra totali. Trovati: {liExtra}";
            return false;
        }

        if (tactics.GreatPerformancePointsTotal > 10)
        {
            error = $"Non si possono usare più di 10 PGP per partita. Trovati: {tactics.GreatPerformancePointsTotal}";
            return false;
        }

        if (tactics.GreatPerformanceDistribution.Values.Sum() != tactics.GreatPerformancePointsTotal)
        {
            error = "La somma dei PGP distribuiti non corrisponde al totale dichiarato.";
            return false;
        }

        if (tactics.HardnessDistribution.Values.Sum() != tactics.HardnessTotal)
        {
            error = "La somma della Durezza distribuita non corrisponde al totale dichiarato.";
            return false;
        }

        if (tactics.TacticianBonusPoints > 0)
        {
            if (tactics.TacticianDistribution.Values.Sum() != 5)
            {
                error = "Il bonus del Tattico deve essere esattamente 5 punti distribuiti su Li/Di/Ce/At.";
                return false;
            }

            if (tactics.TacticianDistribution.ContainsKey("Po"))
            {
                error = "Il bonus del Tattico non può essere usato sul Portiere.";
                return false;
            }
        }

        return true;
    }

    public bool ValidateTactics(Team team, Formation formation, FormationTactics tactics, out string error)
    {
        error = string.Empty;

        var tacticianCount = team.Staff.Count(s => s.Type == BenchStaffType.Tactician);
        if (tacticianCount > 1)
        {
            error = "Una squadra non può avere più di un tattico.";
            return false;
        }

        if (tactics.TacticianBonusPoints > 0)
        {
            var tactician = team.Staff.FirstOrDefault(s => s.Type == BenchStaffType.Tactician);
            if (tactician == null)
            {
                error = "Stai usando punti del Tattico ma la squadra non ha un tattico.";
                return false;
            }

            string currentTactic = TacticHelper.GenerateTacticString(formation, tactics);
            if (!TacticHelper.TacticMatches(tactician.SpecializedTactic, formation, tactics))
            {
                error = $"Il tattico '{tactician.Name}' è specializzato in '{tactician.SpecializedTactic}', ma stai usando la tattica '{currentTactic}'.";
                return false;
            }
        }

        return ValidateTactics(formation, tactics, out error);
    }
}
