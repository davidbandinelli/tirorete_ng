namespace PBEMFootball.Server.Engine;

using PBEMFootball.Common.Models;

public class MatchEngine
{
    private readonly Random _random = new();

    private enum ShotOriginArea
    {
        At,
        Ce
    }

    public void SimulateMatch(Match match)
    {
        if (match.HomeFormation == null || match.AwayFormation == null)
            return;

        match.Events.Clear();
        match.Weather = GenerateWeather();

        if (match.Weather == WeatherCondition.Suspended)
        {
            match.IsPlayed = true;
            return;
        }

        var homeTactics = match.HomeTactics ?? new FormationTactics();
        var awayTactics = match.AwayTactics ?? new FormationTactics();

        var (homePo, homeLi, homeDi, homeCe, homeAt) = CalculateAreaTotalsWithExtras(
            match.HomeFormation, homeTactics, match.IsHomeMatch);
        var (awayPo, awayLi, awayDi, awayCe, awayAt) = CalculateAreaTotalsWithExtras(
            match.AwayFormation, awayTactics, false);

        ApplySideBalancePenalty(match.HomeFormation, ref homeDi, ref homeCe, ref homeAt);
        ApplySideBalancePenalty(match.AwayFormation, ref awayDi, ref awayCe, ref awayAt);

        ApplyTripleRule(ref homeDi, ref homeCe, ref homeAt);
        ApplyTripleRule(ref awayDi, ref awayCe, ref awayAt);

        ProcessHardnessEffects(match, homeTactics.HardnessTotal, awayTactics.HardnessTotal,
            match.HomeFormation, match.AwayFormation);

        ApplyRedCardPenalties(match, match.HomeFormation, ref homePo, ref homeLi, ref homeDi, ref homeCe, ref homeAt);
        ApplyRedCardPenalties(match, match.AwayFormation, ref awayPo, ref awayLi, ref awayDi, ref awayCe, ref awayAt);

        ApplyWeatherEffects(match.Weather, ref homeDi, ref homeCe, ref homeAt, ref awayDi, ref awayCe, ref awayAt);

        var homeShotData = CalculateShots(homeAt, homeCe, homeDi, awayDi, awayCe, awayAt,
            match.HomeFormation.Libero != null, homeTactics.UseCatenaccio, awayTactics.UseOffsideTrap,
            match.AwayFormation.Libero == null);
        var awayShotData = CalculateShots(awayAt, awayCe, awayDi, homeDi, homeCe, homeAt,
            match.AwayFormation.Libero != null, awayTactics.UseCatenaccio, homeTactics.UseOffsideTrap,
            match.HomeFormation.Libero == null);

        var homeShots = homeShotData.total;
        var awayShots = awayShotData.total;

        match.HomeShots = homeShots;
        match.AwayShots = awayShots;

        match.HomeGoals += CalculateGoals(match, homeShots, homeShotData.fromAt, homeShotData.fromCe,
            awayPo, awayLi, match.HomeFormation, match.AwayFormation, true);
        match.AwayGoals += CalculateGoals(match, awayShots, awayShotData.fromAt, awayShotData.fromCe,
            homePo, homeLi, match.AwayFormation, match.HomeFormation, false);

        CalculateOwnGoals(match, match.HomeFormation, match.AwayFormation, awayShots, true);
        CalculateOwnGoals(match, match.AwayFormation, match.HomeFormation, homeShots, false);

        match.IsPlayed = true;
    }

    private WeatherCondition GenerateWeather()
    {
        if (_random.Next(100) < 60)
            return WeatherCondition.Sunny;

        var roll = _random.NextDouble() * 100;
        if (roll < 57.5)
            return WeatherCondition.Covered;
        if (roll < 82.5)
            return WeatherCondition.Rainy;
        if (roll < 95)
            return WeatherCondition.HeavyRain;
        return WeatherCondition.Suspended;
    }

    private (int po, int li, int di, int ce, int at) CalculateAreaTotalsWithExtras(
        Formation formation, FormationTactics tactics, bool isHome)
    {
        var (po, li, di, ce, at) = formation.GetAreaTotals();

        int basePo = po;
        int baseLi = li;

        if (isHome && tactics.HomeFieldAdvantageDistribution.Count > 0)
        {
            if (tactics.HomeFieldAdvantageDistribution.TryGetValue("Di", out int fcDi))
                di += fcDi;
            if (tactics.HomeFieldAdvantageDistribution.TryGetValue("Ce", out int fcCe))
                ce += fcCe;
            if (tactics.HomeFieldAdvantageDistribution.TryGetValue("At", out int fcAt))
                at += fcAt;
        }

        int poExtraTotal = 0;
        int liExtraTotal = 0;

        if (tactics.HardnessDistribution.TryGetValue("Po", out int hardPo))
            poExtraTotal += hardPo;
        if (tactics.HardnessDistribution.TryGetValue("Li", out int hardLi))
            liExtraTotal += hardLi;
        if (tactics.HardnessDistribution.TryGetValue("Di", out int hardDi))
            di += hardDi;
        if (tactics.HardnessDistribution.TryGetValue("Ce", out int hardCe))
            ce += hardCe;
        if (tactics.HardnessDistribution.TryGetValue("At", out int hardAt))
            at += hardAt;

        if (tactics.GreatPerformanceDistribution.TryGetValue("Po", out int pgpPo))
            poExtraTotal += pgpPo;
        if (tactics.GreatPerformanceDistribution.TryGetValue("Li", out int pgpLi))
            liExtraTotal += pgpLi;
        if (tactics.GreatPerformanceDistribution.TryGetValue("Di", out int pgpDi))
            di += pgpDi;
        if (tactics.GreatPerformanceDistribution.TryGetValue("Ce", out int pgpCe))
            ce += pgpCe;
        if (tactics.GreatPerformanceDistribution.TryGetValue("At", out int pgpAt))
            at += pgpAt;

        if (tactics.UseCatenaccio)
        {
            if (tactics.CatenaccioDistribution.TryGetValue("Li", out int catLi))
                liExtraTotal += catLi;
            if (tactics.CatenaccioDistribution.TryGetValue("Di", out int catDi))
                di += catDi;
            if (tactics.CatenaccioDistribution.TryGetValue("Ce", out int catCe))
                ce += catCe;
        }

        if (tactics.TacticianDistribution.Count > 0)
        {
            if (tactics.TacticianDistribution.TryGetValue("Li", out int tacLi))
                liExtraTotal += tacLi;
            if (tactics.TacticianDistribution.TryGetValue("Di", out int tacDi))
                di += tacDi;
            if (tactics.TacticianDistribution.TryGetValue("Ce", out int tacCe))
                ce += tacCe;
            if (tactics.TacticianDistribution.TryGetValue("At", out int tacAt))
                at += tacAt;
        }

        po = basePo + Math.Min(poExtraTotal, 5);
        li = baseLi + Math.Min(liExtraTotal, 5);

        return (po, li, di, ce, at);
    }

    private int GetMaxExtraPoints(int baseValue, int extra, int max)
    {
        var totalExtra = baseValue - (baseValue - extra);
        if (totalExtra > max)
            return baseValue - totalExtra + max;
        return baseValue + extra;
    }

    private void ApplySideBalancePenalty(Formation formation, ref int di, ref int ce, ref int at)
    {
        di -= CalculateSidePenalty(formation.Defenders);
        ce -= CalculateSidePenalty(formation.Midfielders);
        at -= CalculateSidePenalty(formation.Attackers);
    }

    private int CalculateSidePenalty(List<Player> players)
    {
        if (players.Count == 0)
            return 0;

        int leftCount = 0;
        int rightCount = 0;

        foreach (var player in players)
        {
            if (player.Side == PlayerSide.SD)
                continue;
            if (player.Side == PlayerSide.S)
                leftCount++;
            else
                rightCount++;
        }

        int idealSplit = players.Count / 2;
        int imbalance = Math.Abs(leftCount - rightCount) - (players.Count % 2);
        return Math.Max(0, imbalance) * 3;
    }

    private void ApplyTripleRule(ref int di, ref int ce, ref int at)
    {
        int minArea = Math.Min(Math.Min(di, ce), at);
        int maxAllowed = minArea * 3;

        if (di > maxAllowed)
            di = maxAllowed;
        if (ce > maxAllowed)
            ce = maxAllowed;
        if (at > maxAllowed)
            at = maxAllowed;
    }

    private (int total, int fromAt, int fromCe) CalculateShots(int at, int ce, int di, int defDi, int defCe, int defAt,
        bool hasLibero, bool useCatenaccio, bool opponentUsesOffsideTrap, bool opponentHasNoLibero)
    {
        int shotsFromAt = Math.Max(0, at - defDi);
        int shotsFromCe = Math.Max(0, (ce - defCe) / 2);

        if (di > defAt)
        {
            if (hasLibero)
                shotsFromCe += (di - defAt) / 3;
            else
                shotsFromCe += (di - defAt) / 5;
        }

        if (opponentUsesOffsideTrap && opponentHasNoLibero)
        {
            shotsFromAt = shotsFromAt / 2;
            shotsFromCe = shotsFromCe * 2;
        }

        int sourceTotal = shotsFromAt + shotsFromCe;
        int shots = sourceTotal;

        if (useCatenaccio)
            shots = shots / 2;

        if (shots <= 0 || sourceTotal <= 0)
            return (0, 0, 0);

        int fromAt = (shots * shotsFromAt) / sourceTotal;
        int fromCe = shots - fromAt;

        return (shots, fromAt, fromCe);
    }

    private int CalculateGoals(Match match, int shots, int shotsFromAt, int shotsFromCe,
        int po, int li, Formation attackingFormation,
        Formation defendingFormation, bool isHomeAttacking)
    {
        int goals = 0;
        int shotNumber = 0;
        int remainingFromAt = shotsFromAt;
        int remainingFromCe = shotsFromCe;

        for (int i = 0; i < shots; i++)
        {
            shotNumber++;
            var shotOrigin = GetShotOrigin(ref remainingFromAt, ref remainingFromCe);
            int missChance = 30;
            if (goals >= 5)
                missChance = 70;
            else if (goals >= 3)
                missChance = 50;

            if (_random.Next(100) < missChance)
            {
                match.Events.Add(new MatchEvent
                {
                    Type = MatchEventType.ShotMissed,
                    Player = null,
                    IsHomeTeam = isHomeAttacking,
                    Minute = _random.Next(1, 91),
                    Description = $"Tiro #{shotNumber}: FUORI (prob. errore: {missChance}%)"
                });

                if (_random.Next(100) < 5)
                {
                }
                continue;
            }

            int liberoBlockChance = 25 + li * 2;
            if (li > 0 && _random.Next(100) < liberoBlockChance)
            {
                var liberoName = defendingFormation.Libero?.Name ?? "Libero";
                match.Events.Add(new MatchEvent
                {
                    Type = MatchEventType.ShotBlockedByLibero,
                    Player = defendingFormation.Libero,
                    IsHomeTeam = isHomeAttacking,
                    Minute = _random.Next(1, 91),
                    Description = $"Tiro #{shotNumber}: INTERCETTATO dal Libero {liberoName} (prob: {liberoBlockChance}%)"
                });
                continue;
            }

            int goalkeeperSaveChance = (int)(35 + po * 2.25);
            if (_random.Next(100) < goalkeeperSaveChance)
            {
                var goalkeeperName = defendingFormation.Goalkeeper?.Name ?? "Portiere";
                match.Events.Add(new MatchEvent
                {
                    Type = MatchEventType.ShotSavedByGoalkeeper,
                    Player = defendingFormation.Goalkeeper,
                    IsHomeTeam = isHomeAttacking,
                    Minute = _random.Next(1, 91),
                    Description = $"Tiro #{shotNumber}: PARATO dal Portiere {goalkeeperName} (prob: {goalkeeperSaveChance}%)"
                });
                continue;
            }

            goals++;
            var scorer = SelectGoalScorer(attackingFormation, shotOrigin);
            match.Events.Add(new MatchEvent
            {
                Type = MatchEventType.Goal,
                Player = scorer,
                IsHomeTeam = isHomeAttacking,
                Minute = _random.Next(1, 91),
                Description = scorer != null
                    ? $"Tiro #{shotNumber}: GOL di {scorer.Name}!"
                    : $"Tiro #{shotNumber}: GOL!"
            });
        }

        return goals;
    }

    private ShotOriginArea GetShotOrigin(ref int remainingFromAt, ref int remainingFromCe)
    {
        if (remainingFromAt <= 0 && remainingFromCe <= 0)
            return ShotOriginArea.At;

        if (remainingFromAt <= 0)
        {
            remainingFromCe--;
            return ShotOriginArea.Ce;
        }

        if (remainingFromCe <= 0)
        {
            remainingFromAt--;
            return ShotOriginArea.At;
        }

        int total = remainingFromAt + remainingFromCe;
        if (_random.Next(total) < remainingFromAt)
        {
            remainingFromAt--;
            return ShotOriginArea.At;
        }

        remainingFromCe--;
        return ShotOriginArea.Ce;
    }

    private Player? SelectGoalScorer(Formation attackingFormation, ShotOriginArea shotOrigin)
    {
        var allPlayers = new List<Player>();
        if (attackingFormation.Goalkeeper != null)
            allPlayers.Add(attackingFormation.Goalkeeper);
        if (attackingFormation.Libero != null)
            allPlayers.Add(attackingFormation.Libero);
        allPlayers.AddRange(attackingFormation.Defenders);
        allPlayers.AddRange(attackingFormation.Midfielders);
        allPlayers.AddRange(attackingFormation.Attackers);

        if (allPlayers.Count == 0)
            return null;

        Player? bestPlayer = null;
        double bestRoll = double.MinValue;

        foreach (var player in allPlayers)
        {
            double maxRoll;
            if (player.Position == PlayerPosition.Po)
            {
                maxRoll = 3;
            }
            else
            {
                double baseMax = Math.Max(0, player.Ability + (3 * player.Form));
                bool isInShotArea = shotOrigin switch
                {
                    ShotOriginArea.At => attackingFormation.Attackers.Contains(player),
                    ShotOriginArea.Ce => attackingFormation.Midfielders.Contains(player),
                    _ => false
                };

                maxRoll = isInShotArea ? baseMax : baseMax / 1.75;
            }

            double roll = maxRoll > 0 ? _random.NextDouble() * maxRoll : 0;
            if (roll > bestRoll)
            {
                bestRoll = roll;
                bestPlayer = player;
            }
        }

        return bestPlayer;
    }

    private void CalculateOwnGoals(Match match, Formation defending, Formation attacking,
        int opponentShots, bool isHomeDefending)
    {
        int defenders = defending.Defenders.Count + (defending.Libero != null ? 1 : 0);
        int midfielders = defending.Midfielders.Count;

        double deflectionChance = (defenders + midfielders / 2.0) / 10.0;

        for (int i = 0; i < opponentShots; i++)
        {
            if (_random.NextDouble() < deflectionChance)
            {
                var allDefensivePlayers = new List<Player>();
                allDefensivePlayers.AddRange(defending.Defenders);
                allDefensivePlayers.AddRange(defending.Midfielders);
                if (defending.Libero != null)
                    allDefensivePlayers.Add(defending.Libero);

                if (allDefensivePlayers.Count > 0)
                {
                    var deflector = allDefensivePlayers[_random.Next(allDefensivePlayers.Count)];
                    double ownGoalChance = deflector.Age switch
                    {
                        PlayerAge.Primavera => 0.009,
                        PlayerAge.Juniores => 0.009,
                        PlayerAge.I => 0.009,
                        PlayerAge.II => 0.006,
                        PlayerAge.III => 0.003,
                        _ => 0.0005
                    };

                    if (_random.NextDouble() < ownGoalChance)
                    {
                        if (isHomeDefending)
                            match.HomeGoals++;
                        else
                            match.AwayGoals++;

                        match.Events.Add(new MatchEvent
                        {
                            Type = MatchEventType.OwnGoal,
                            Player = deflector,
                            IsHomeTeam = isHomeDefending,
                            Minute = _random.Next(1, 91),
                            Description = $"AUTOGOAL di {deflector.Name}!"
                        });
                    }
                }
            }
        }
    }

    private void ProcessHardnessEffects(Match match, int homeHardness, int awayHardness,
        Formation homeFormation, Formation awayFormation)
    {
        ProcessTeamHardness(match, homeHardness, homeFormation, true, awayFormation, match.HomeTactics);
        ProcessTeamHardness(match, awayHardness, awayFormation, false, homeFormation, match.AwayTactics);
    }

    private void ProcessTeamHardness(Match match, int hardness, Formation formation, 
        bool isHomeTeam, Formation opponentFormation, FormationTactics? tactics)
    {
        var allPlayers = new List<Player>();
        if (formation.Goalkeeper != null)
            allPlayers.Add(formation.Goalkeeper);
        if (formation.Libero != null)
            allPlayers.Add(formation.Libero);
        allPlayers.AddRange(formation.Defenders);
        allPlayers.AddRange(formation.Midfielders);
        allPlayers.AddRange(formation.Attackers);

        foreach (var player in allPlayers)
        {
            int playerHardness = hardness;
            if (player.Position == PlayerPosition.Po && tactics != null)
            {
                playerHardness = tactics.HardnessDistribution.GetValueOrDefault("Po", 0);
            }

            double yellowChance = 1.5 * (3 + playerHardness);
            if (_random.NextDouble() * 100 < yellowChance)
            {
                player.DisciplinePoints += 4;
                match.Events.Add(new MatchEvent
                {
                    Type = MatchEventType.YellowCard,
                    Player = player,
                    IsHomeTeam = isHomeTeam,
                    Minute = _random.Next(1, 91),
                    Description = $"Ammonito: {player.Name}"
                });
            }

            double redChance = 0.33 * (3 + playerHardness);
            if (_random.NextDouble() * 100 < redChance)
            {
                player.DisciplinePoints += 10;
                match.Events.Add(new MatchEvent
                {
                    Type = MatchEventType.RedCard,
                    Player = player,
                    IsHomeTeam = isHomeTeam,
                    Minute = _random.Next(1, 91),
                    Description = $"ESPULSO: {player.Name}"
                });
            }
        }

        var opponentPlayers = new List<Player>();
        if (opponentFormation.Goalkeeper != null)
            opponentPlayers.Add(opponentFormation.Goalkeeper);
        if (opponentFormation.Libero != null)
            opponentPlayers.Add(opponentFormation.Libero);
        opponentPlayers.AddRange(opponentFormation.Defenders);
        opponentPlayers.AddRange(opponentFormation.Midfielders);
        opponentPlayers.AddRange(opponentFormation.Attackers);

        Team opponentTeam = isHomeTeam ? match.AwayTeam : match.HomeTeam;
        bool hasMasseur = opponentTeam.Staff.Any(s => s.Type == BenchStaffType.Masseur);

        foreach (var opponent in opponentPlayers)
        {
            double injuryChance = 5 + hardness;
            if (opponent.Position == PlayerPosition.Po)
                injuryChance /= 2;

            if (_random.NextDouble() * 100 < injuryChance)
            {
                int severity = _random.Next(1, 101);

                if (hasMasseur && severity > 15)
                {
                    if (severity <= 30)
                        severity = 15;
                    else if (severity <= 45)
                        severity = 30;
                    else if (severity <= 60)
                        severity = 45;
                    else if (severity <= 70)
                        severity = 60;
                    else if (severity <= 80)
                        severity = 70;
                    else if (severity <= 90)
                        severity = 80;
                    else
                        severity = 90;
                }

                int formLoss = severity switch
                {
                    <= 15 => 0,
                    <= 30 => 0,
                    <= 45 => 1,
                    <= 60 => 2,
                    <= 70 => 3,
                    <= 80 => 4,
                    <= 90 => 5,
                    _ => 6
                };

                string matchLevel = severity switch
                {
                    <= 15 => "3/4",
                    _ => "1/2"
                };

                opponent.Form -= formLoss;

                string masseurNote = hasMasseur ? " (ridotto da Massaggiatore)" : "";
                match.Events.Add(new MatchEvent
                {
                    Type = MatchEventType.Injury,
                    Player = opponent,
                    IsHomeTeam = !isHomeTeam,
                    Minute = _random.Next(1, 91),
                    Description = $"INFORTUNIO: {opponent.Name} - Livello in partita: {matchLevel}, Fo -{formLoss}{masseurNote}"
                });
            }
        }

        int penaltyRolls = Math.Max(hardness, 1);
        int penaltyChance = hardness == 0 ? 5 : 10;

        for (int i = 0; i < penaltyRolls; i++)
        {
            if (_random.Next(100) < penaltyChance)
            {
                var bestOpponent = opponentPlayers
                    .Where(p => p.Position != PlayerPosition.Po && p.Form > -3 && p.DisciplinePoints < 10)
                    .OrderByDescending(p => p.Ability + p.Form)
                    .FirstOrDefault();

                if (bestOpponent != null)
                {
                    int opponentPo = opponentFormation.Goalkeeper != null ? 
                        opponentFormation.Goalkeeper.Ability : 0;
                    int tiratorAb = bestOpponent.Ability;
                    int scoreProbability = Math.Min(95, 40 + (3 * tiratorAb) - opponentPo);

                    bool scored = _random.Next(100) < scoreProbability;

                    if (scored)
                    {
                        if (isHomeTeam)
                            match.AwayGoals++;
                        else
                            match.HomeGoals++;

                        match.Events.Add(new MatchEvent
                        {
                            Type = MatchEventType.Penalty,
                            Player = bestOpponent,
                            IsHomeTeam = !isHomeTeam,
                            Minute = _random.Next(1, 91),
                            Description = $"RIGORE segnato da {bestOpponent.Name}! (prob: {scoreProbability}%)"
                        });
                    }
                    else
                    {
                        var goalkeeperName = opponentFormation.Goalkeeper?.Name ?? "Portiere";
                        match.Events.Add(new MatchEvent
                        {
                            Type = MatchEventType.PenaltyMissed,
                            Player = bestOpponent,
                            IsHomeTeam = !isHomeTeam,
                            Minute = _random.Next(1, 91),
                            Description = $"RIGORE parato da {goalkeeperName} su tiro di {bestOpponent.Name} (prob: {scoreProbability}%)"
                        });
                    }
                }
            }
        }
    }

    private void ApplyRedCardPenalties(Match match, Formation formation, ref int po, ref int li, ref int di, ref int ce, ref int at)
    {
        var redCardedPlayers = match.Events
            .Where(e => e.Type == MatchEventType.RedCard && 
                   (e.Player == formation.Goalkeeper || 
                    e.Player == formation.Libero ||
                    formation.Defenders.Contains(e.Player) ||
                    formation.Midfielders.Contains(e.Player) ||
                    formation.Attackers.Contains(e.Player)))
            .Select(e => e.Player)
            .ToList();

        foreach (var player in redCardedPlayers)
        {
            int playerValue = (player.Ability + player.Form) / 2;

            if (player == formation.Goalkeeper)
                po -= playerValue;
            else if (player == formation.Libero)
                li -= playerValue;
            else if (formation.Defenders.Contains(player))
                di -= playerValue;
            else if (formation.Midfielders.Contains(player))
                ce -= playerValue;
            else if (formation.Attackers.Contains(player))
                at -= playerValue;
        }

        po = Math.Max(0, po);
        li = Math.Max(0, li);
        di = Math.Max(0, di);
        ce = Math.Max(0, ce);
        at = Math.Max(0, at);
    }

    private void ApplyWeatherEffects(WeatherCondition weather, ref int homeDi, ref int homeCe,
        ref int homeAt, ref int awayDi, ref int awayCe, ref int awayAt)
    {
        if (weather == WeatherCondition.Rainy)
        {
            homeAt = (homeAt * 4) / 5;
            awayAt = (awayAt * 4) / 5;
        }
        else if (weather == WeatherCondition.HeavyRain)
        {
            homeAt = (homeAt * 2) / 3;
            awayAt = (awayAt * 2) / 3;
            homeCe = (homeCe * 4) / 5;
            awayCe = (awayCe * 4) / 5;
            homeDi = (homeDi * 4) / 5;
            awayDi = (awayDi * 4) / 5;
        }
    }
}