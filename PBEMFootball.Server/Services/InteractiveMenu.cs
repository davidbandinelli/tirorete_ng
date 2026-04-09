namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;
using PBEMFootball.Common.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

public class InteractiveMenu
{
    private readonly SeasonManager _seasonManager;
    private readonly TeamFactory _teamFactory;
    private readonly Random _random = new();
    private readonly HashSet<string> _generatedPlayerNames = new(StringComparer.OrdinalIgnoreCase);
    private Season? _currentSeason;
    private List<Team> _allTeams = new();

    private static readonly List<(int def, int mid, int att)> AllowedModules =
    [
        (4, 4, 2),
        (4, 3, 3),
        (4, 2, 4),
        (3, 2, 5),
        (3, 5, 2),
        (6, 2, 2),
        (2, 6, 2),
        (2, 2, 6),
        (3, 4, 3),
        (3, 3, 4),
        (2, 4, 4)
    ];

    public InteractiveMenu()
    {
        _seasonManager = new SeasonManager();
        _teamFactory = new TeamFactory();
    }

    private const int TABLE_WIDTH = 72; // Caratteri tra ║ e ║

    /// <summary>
    /// Crea una riga di tabella con padding automatico per mantenere larghezza fissa
    /// </summary>
    private string CreateTableRow(string content)
    {
        int contentLength = content.Length;
        int padding = TABLE_WIDTH - contentLength;

        if (padding < 0)
        {
            // Se il contenuto è troppo lungo, tronca
            content = content.Substring(0, TABLE_WIDTH);
            padding = 0;
        }

        return $"║{content}{new string(' ', padding)}║";
    }

    public void Run()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    PBEM FOOTBALL - MENU PRINCIPALE                     ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. Inserisci dati squadre partecipanti                                 ║");
            Console.WriteLine("║ 2. Visualizza tutte le squadre                                         ║");
            Console.WriteLine("║ 3. Crea nuova stagione                                                 ║");
            Console.WriteLine("║ 4. Visualizza classifica campionato                                    ║");
            Console.WriteLine("║ 5. Visualizza classifica marcatori                                     ║");
            Console.WriteLine("║ 6. Simula una giornata                                                 ║");
            Console.WriteLine("║ 7. Gestisci Punti Speciali (PS)                                        ║");
            Console.WriteLine("║ 8. Genera 12 squadre casuali                                           ║");
            Console.WriteLine("║ 9. Salva squadre in JSON                                               ║");
            Console.WriteLine("║ 10. Carica squadre da JSON                                             ║");
            Console.WriteLine("║ 11. Visualizza dettagli squadra                                        ║");
            Console.WriteLine("║ 12. Visualizza calendario competizioni                                 ║");
            Console.WriteLine("║ 0. Esci                                                                ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.Write("\nScegli un'opzione: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    InsertTeamsData();
                    break;
                case "2":
                    DisplayAllTeams();
                    break;
                case "3":
                    CreateNewSeason();
                    break;
                case "4":
                    DisplayChampionshipStandings();
                    break;
                case "5":
                    DisplayTopScorers();
                    break;
                case "6":
                    SimulateRound();
                    break;
                case "7":
                    ManageSpecialPoints();
                    break;
                case "8":
                    GenerateRandomTeams();
                    break;
                case "9":
                    SaveTeamsToJson();
                    break;
                case "10":
                    LoadTeamsFromJson();
                    break;
                case "11":
                    DisplayTeamDetails();
                    break;
                case "12":
                    DisplayCalendar();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Opzione non valida!");
                    break;
            }

            if (!exit && choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5" && choice != "6" && choice != "7" && choice != "8" && choice != "9" && choice != "10" && choice != "11" && choice != "12")
            {
                Console.WriteLine("\nPremi un tasto per continuare...");
                Console.ReadKey();
            }
        }
    }

    private void InsertTeamsData()
    {
        Console.Clear();
        Console.WriteLine("=== INSERIMENTO DATI SQUADRE ===\n");
        
        Console.Write("Numero di squadre (default 12): ");
        string? input = Console.ReadLine();
        int teamCount = string.IsNullOrWhiteSpace(input) ? 12 : int.Parse(input);

        _allTeams.Clear();

        for (int i = 1; i <= teamCount; i++)
        {
            Console.WriteLine($"\n--- Squadra {i} ---");
            
            Console.Write("Nome squadra: ");
            string? teamName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(teamName)) teamName = $"Squadra {i}";

            Console.Write("Nome presidente/manager: ");
            string? managerName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(managerName)) managerName = $"Manager {i}";

            Console.Write("PA (Punti Allenamento, default 5): ");
            string? paInput = Console.ReadLine();
            int pa = string.IsNullOrWhiteSpace(paInput) ? 5 : int.Parse(paInput);

            Console.Write("PGP (Punti Grande Prestazione, default 30): ");
            string? pgpInput = Console.ReadLine();
            int pgp = string.IsNullOrWhiteSpace(pgpInput) ? 30 : int.Parse(pgpInput);

            Console.Write("M (Milioni, default 0): ");
            string? moneyInput = Console.ReadLine();
            int money = string.IsNullOrWhiteSpace(moneyInput) ? 0 : int.Parse(moneyInput);

            Console.Write("PS (Punti Speciali, default 0): ");
            string? psInput = Console.ReadLine();
            int ps = string.IsNullOrWhiteSpace(psInput) ? 0 : int.Parse(psInput);

            var team = _teamFactory.CreateInitialTeam(teamName, managerName);
            team.TrainingPoints = pa;
            team.GreatPerformancePoints = pgp;
            team.Money = money;
            team.SpecialPoints = ps;

            _allTeams.Add(team);
            Console.WriteLine($"✓ Squadra '{teamName}' aggiunta!");
        }

        Console.WriteLine($"\n✓ Totale {_allTeams.Count} squadre inserite!");
        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void DisplayAllTeams()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow(" SQUADRE PARTECIPANTI "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        if (_allTeams.Count == 0)
        {
            Console.WriteLine(CreateTableRow(" Nessuna squadra inserita. Usa l'opzione 1 per inserire i dati. "));
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
        }
        else
        {
            Console.WriteLine(CreateTableRow(" N.  Squadra              Manager              PA  PGP    M   PS "));
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

            for (int i = 0; i < _allTeams.Count; i++)
            {
                var team = _allTeams[i];
                string content = $" {(i + 1),2}. {team.Name,-20} {team.ManagerName,-20} {team.TrainingPoints,3} {team.GreatPerformancePoints,4} {team.Money,4} {team.SpecialPoints,4} ";
                Console.WriteLine(CreateTableRow(content));
            }

            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"\nTotale squadre: {_allTeams.Count}");
        }

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void CreateNewSeason()
    {
        Console.Clear();
        Console.WriteLine("=== CREAZIONE NUOVA STAGIONE ===\n");

        if (_allTeams.Count < 2)
        {
            Console.WriteLine("Errore: servono almeno 2 squadre per creare una stagione!");
            Console.WriteLine("Usa l'opzione 1 per inserire le squadre o l'opzione 8 per generarle.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        if (_allTeams.Count % 2 != 0)
        {
            Console.WriteLine($"Errore: il numero di squadre deve essere pari per il calendario Round-Robin!");
            Console.WriteLine($"Squadre attuali: {_allTeams.Count}");
            Console.WriteLine("Aggiungi o rimuovi una squadra per avere un numero pari.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        Console.Write("Anno della stagione (default 2024): ");
        string? yearInput = Console.ReadLine();
        int year = string.IsNullOrWhiteSpace(yearInput) ? 2024 : int.Parse(yearInput);

        // Per semplicità, tutte le squadre in Serie A
        var serieATeams = _allTeams;
        var serieBTeams = new List<Team>();
        var serieCTeams = new List<Team>();

        try
        {
            _currentSeason = _seasonManager.CreateSeason(year, serieATeams, serieBTeams, serieCTeams);

            Console.WriteLine($"\n✓ Stagione {year} creata con successo!");
            Console.WriteLine($"  - Squadre in Serie A: {serieATeams.Count}");
            Console.WriteLine($"  - Competizioni create: {_currentSeason.Competitions.Count}");
            Console.WriteLine($"  - Sessioni di gioco: {_currentSeason.Sessions.Count}");
            Console.WriteLine($"  - Partite totali: {_currentSeason.Sessions.Sum(s => s.Matches.Count)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERRORE] Impossibile creare la stagione: {ex.Message}");
        }

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void DisplayChampionshipStandings()
    {
        Console.Clear();

        if (_currentSeason == null)
        {
            Console.WriteLine("Errore: nessuna stagione attiva. Usa l'opzione 3 per creare una stagione.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        var championship = _currentSeason.GetChampionship(Division.SerieA);
        if (championship == null)
        {
            Console.WriteLine("Errore: campionato non trovato.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow(" CLASSIFICA CAMPIONATO "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine(CreateTableRow(" Pos  Squadra                G   V  P  S   GF  GS  DR  Punti "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        int position = 1;
        foreach (var standing in championship.Standings)
        {
            string content = $" {position,2}.  {standing.Team.Name,-20} " +
                          $"{standing.Played,2}  {standing.Won,2} {standing.Drawn,2} {standing.Lost,2}  " +
                          $"{standing.GoalsFor,3} {standing.GoalsAgainst,3} {standing.GoalDifference,4} " +
                          $"{standing.Points,3} ";
            Console.WriteLine(CreateTableRow(content));
            position++;
        }

        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine("\nLegenda: G=Giocate, V=Vinte, P=Pareggiate, S=Sconfitte");
        Console.WriteLine("         GF=Goal Fatti, GS=Goal Subiti, DR=Differenza Reti");

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void DisplayTopScorers()
    {
        Console.Clear();

        if (_currentSeason == null)
        {
            Console.WriteLine("Errore: nessuna stagione attiva.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        var championship = _currentSeason.GetChampionship(Division.SerieA);
        if (championship == null)
        {
            Console.WriteLine("Errore: campionato non trovato.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        // Raccoglie tutti i goal e penalty segnati
        var scorers = new Dictionary<Player, int>();

        foreach (var match in championship.Matches.Where(m => m.IsPlayed))
        {
            var goalEvents = match.Events.Where(e => 
                e.Type == MatchEventType.Goal || 
                e.Type == MatchEventType.Penalty);

            foreach (var goalEvent in goalEvents)
            {
                if (goalEvent.Player != null)
                {
                    if (!scorers.ContainsKey(goalEvent.Player))
                        scorers[goalEvent.Player] = 0;
                    scorers[goalEvent.Player]++;
                }
            }
        }

        var topScorers = scorers.OrderByDescending(s => s.Value).Take(20).ToList();

        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow(" CLASSIFICA MARCATORI "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine(CreateTableRow(" Pos  Giocatore           Squadra              Goal "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        if (topScorers.Count == 0)
        {
            Console.WriteLine(CreateTableRow(" Nessun goal segnato ancora. "));
        }
        else
        {
            int position = 1;
            foreach (var scorer in topScorers)
            {
                var team = _allTeams.FirstOrDefault(t => t.Players.Contains(scorer.Key));
                string teamName = team?.Name ?? "N/A";

                string content = $" {position,2}.  {scorer.Key.Name,-20} {teamName,-20} {scorer.Value,4} ";
                Console.WriteLine(CreateTableRow(content));
                position++;
            }
        }

        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void SimulateRound()
    {
        Console.Clear();
        Console.WriteLine("=== SIMULA GIORNATA ===\n");

        if (_currentSeason == null)
        {
            Console.WriteLine("Errore: nessuna stagione attiva.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        var championship = _currentSeason.GetChampionship(Division.SerieA);
        if (championship == null)
        {
            Console.WriteLine("Errore: campionato non trovato.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        // Trova la prossima giornata da giocare
        int nextRound = championship.CurrentRound + 1;
        var roundMatches = championship.Matches
            .Where(m => m.Round == nextRound && !m.IsPlayed)
            .ToList();

        if (roundMatches.Count == 0)
        {
            Console.WriteLine("Non ci sono più giornate da giocare!");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Giornata {nextRound}:\n");

        var matchEngine = new Engine.MatchEngine();

        foreach (var match in roundMatches)
        {
            var trackedHomeTeam = ResolveTrackedTeam(match.HomeTeam);
            var trackedAwayTeam = ResolveTrackedTeam(match.AwayTeam);

            var homeSuspendedBeforeMatch = trackedHomeTeam.Players
                .Where(p => p.DisciplinePoints >= 10)
                .ToList();
            var awaySuspendedBeforeMatch = trackedAwayTeam.Players
                .Where(p => p.DisciplinePoints >= 10)
                .ToList();

            var (homeFormation, homeTactics, homeModule) = CreateRandomFormationAndTactics(match.HomeTeam, null);
            var (awayFormation, awayTactics, _) = CreateRandomFormationAndTactics(match.AwayTeam, homeModule);

            ApplyHomeFieldAdvantage(homeTactics);
            ApplyGreatPerformancePoints(trackedHomeTeam, homeFormation, homeTactics);
            ApplyGreatPerformancePoints(trackedAwayTeam, awayFormation, awayTactics);
            ApplyTacticianBonus(trackedHomeTeam, homeFormation, homeTactics);
            ApplyTacticianBonus(trackedAwayTeam, awayFormation, awayTactics);

            match.HomeFormation = homeFormation;
            match.AwayFormation = awayFormation;
            match.HomeTactics = homeTactics;
            match.AwayTactics = awayTactics;

            matchEngine.SimulateMatch(match);

            SynchronizeTeamPlayerSeasonStats(match.HomeTeam, match.HomeFormation);
            SynchronizeTeamPlayerSeasonStats(match.AwayTeam, match.AwayFormation);

            foreach (var suspendedPlayer in homeSuspendedBeforeMatch)
            {
                suspendedPlayer.DisciplinePoints = Math.Max(0, suspendedPlayer.DisciplinePoints - 10);
            }

            foreach (var suspendedPlayer in awaySuspendedBeforeMatch)
            {
                suspendedPlayer.DisciplinePoints = Math.Max(0, suspendedPlayer.DisciplinePoints - 10);
            }

            Console.WriteLine($"{match.HomeTeam.Name,-20} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam.Name,-20}");
            DisplayMatchEvents(match);

            // Aggiorna classifica
            _seasonManager.UpdateStandings(championship, match);
        }

        championship.CurrentRound = nextRound;

        Console.WriteLine($"\n✓ Giornata {nextRound} completata!");
        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private Team ResolveTrackedTeam(Team team)
    {
        return _allTeams.FirstOrDefault(t => ReferenceEquals(t, team))
            ?? _allTeams.FirstOrDefault(t => t.Name == team.Name && t.ManagerName == team.ManagerName)
            ?? team;
    }

    private void DisplayMatchEvents(Match match)
    {
        Console.WriteLine($"  Tiri: {match.HomeTeam.Name} {match.HomeShots} - {match.AwayShots} {match.AwayTeam.Name}");
        Console.WriteLine($"  Tattica {match.HomeTeam.Name}: {GetTacticDescription(match.HomeFormation, match.HomeTactics)}");
        Console.WriteLine($"  Tattica {match.AwayTeam.Name}: {GetTacticDescription(match.AwayFormation, match.AwayTactics)}");
        PrintTeamAreaSummary(match.HomeTeam.Name, match.HomeAreaSummary);
        PrintTeamAreaSummary(match.AwayTeam.Name, match.AwayAreaSummary);

        var orderedEvents = match.Events
            .OrderBy(e => e.Minute)
            .ToList();

        if (orderedEvents.Count == 0)
        {
            Console.WriteLine("  Nessun evento registrato.");
            Console.WriteLine();
            return;
        }

        var disciplinaryEvents = orderedEvents
            .Where(e => e.Type == MatchEventType.YellowCard || e.Type == MatchEventType.RedCard)
            .ToList();

        var injuryEvents = orderedEvents
            .Where(e => e.Type == MatchEventType.Injury)
            .ToList();

        var matchEvents = orderedEvents
            .Where(e => e.Type != MatchEventType.YellowCard &&
                        e.Type != MatchEventType.RedCard &&
                        e.Type != MatchEventType.Injury)
            .ToList();

        Console.WriteLine("  Eventi partita:");
        PrintEventList(matchEvents, match.HomeTeam.Name, match.AwayTeam.Name);

        Console.WriteLine("  Eventi disciplinari:");
        PrintEventList(disciplinaryEvents, match.HomeTeam.Name, match.AwayTeam.Name);

        Console.WriteLine("  Infortuni:");
        PrintEventList(injuryEvents, match.HomeTeam.Name, match.AwayTeam.Name);
        Console.WriteLine();
    }

    private void PrintTeamAreaSummary(string teamName, TeamMatchAreaSummary? summary)
    {
        if (summary == null)
            return;

        Console.WriteLine($"  FC {teamName}: Di={summary.HomeFieldDistribution.GetValueOrDefault("Di", 0)} Ce={summary.HomeFieldDistribution.GetValueOrDefault("Ce", 0)} At={summary.HomeFieldDistribution.GetValueOrDefault("At", 0)}");
        Console.WriteLine($"  D  {teamName}: Po={summary.HardnessDistribution.GetValueOrDefault("Po", 0)} Li={summary.HardnessDistribution.GetValueOrDefault("Li", 0)} Di={summary.HardnessDistribution.GetValueOrDefault("Di", 0)} Ce={summary.HardnessDistribution.GetValueOrDefault("Ce", 0)} At={summary.HardnessDistribution.GetValueOrDefault("At", 0)}");
        Console.WriteLine($"  PGP {teamName}: Po={summary.GreatPerformanceDistribution.GetValueOrDefault("Po", 0)} Li={summary.GreatPerformanceDistribution.GetValueOrDefault("Li", 0)} Di={summary.GreatPerformanceDistribution.GetValueOrDefault("Di", 0)} Ce={summary.GreatPerformanceDistribution.GetValueOrDefault("Ce", 0)} At={summary.GreatPerformanceDistribution.GetValueOrDefault("At", 0)}");
        if (summary.TacticianBonusPoints > 0)
        {
            Console.WriteLine($"  TAC {teamName}: Li={summary.TacticianDistribution.GetValueOrDefault("Li", 0)} Di={summary.TacticianDistribution.GetValueOrDefault("Di", 0)} Ce={summary.TacticianDistribution.GetValueOrDefault("Ce", 0)} At={summary.TacticianDistribution.GetValueOrDefault("At", 0)} (Totale: {summary.TacticianBonusPoints})");
        }
        Console.WriteLine($"  Totali aree {teamName}: Po={summary.Po} Li={summary.Li} Di={summary.Di} Ce={summary.Ce} At={summary.At}");
    }

    private void PrintEventList(List<MatchEvent> events, string homeTeamName, string awayTeamName)
    {
        if (events.Count == 0)
        {
            Console.WriteLine("    - Nessuno");
            return;
        }

        foreach (var matchEvent in events)
        {
            string team = matchEvent.IsHomeTeam ? homeTeamName : awayTeamName;
            string playerName = matchEvent.Player?.Name ?? "N/A";
            bool isPenaltyEvent = matchEvent.Type == MatchEventType.Penalty ||
                                  matchEvent.Type == MatchEventType.PenaltyMissed;

            if (isPenaltyEvent)
            {
                Console.WriteLine($"    - {matchEvent.Minute,2}' [{team}] {matchEvent.Type} - Tiratore: {playerName} | {matchEvent.Description}");
            }
            else
            {
                Console.WriteLine($"    - {matchEvent.Minute,2}' [{team}] {matchEvent.Type} - {playerName}: {matchEvent.Description}");
            }
        }
    }

    private string GetTacticDescription(Formation? formation, FormationTactics? tactics)
    {
        if (formation == null)
            return "N/D";

        string module = $"{formation.Defenders.Count}-{formation.Midfielders.Count}-{formation.Attackers.Count}";
        var details = new List<string>();

        if (formation.Libero != null)
            details.Add("con Libero");

        if (tactics?.UseOffsideTrap == true)
            details.Add("con TFG (trappola fuori gioco)");

        if (tactics?.UseCatenaccio == true)
            details.Add("CAT (con catenaccio)");

        return details.Count == 0
            ? module
            : $"{module} {string.Join(", ", details)}";
    }

    private void SynchronizeTeamPlayerSeasonStats(Team team, Formation? formation)
    {
        if (formation == null)
            return;

        var targetTeam = _allTeams.FirstOrDefault(t => ReferenceEquals(t, team))
            ?? _allTeams.FirstOrDefault(t => t.Name == team.Name && t.ManagerName == team.ManagerName)
            ?? team;

        var playersInMatch = new List<Player>();
        if (formation.Goalkeeper != null)
            playersInMatch.Add(formation.Goalkeeper);
        if (formation.Libero != null)
            playersInMatch.Add(formation.Libero);
        playersInMatch.AddRange(formation.Defenders);
        playersInMatch.AddRange(formation.Midfielders);
        playersInMatch.AddRange(formation.Attackers);

        foreach (var matchPlayer in playersInMatch)
        {
            var teamPlayer = targetTeam.Players.FirstOrDefault(p => ReferenceEquals(p, matchPlayer))
                ?? targetTeam.Players.FirstOrDefault(p => p.Name == matchPlayer.Name && p.Position == matchPlayer.Position);

            if (teamPlayer == null)
                continue;

            teamPlayer.Ability = matchPlayer.Ability;
            teamPlayer.Form = matchPlayer.Form;
            teamPlayer.DisciplinePoints = matchPlayer.DisciplinePoints;
            teamPlayer.MatchesPlayedThisSession++;
        }
    }

    private Formation CreateDefaultFormation(Team team)
    {
        var availablePlayers = team.Players
            .Where(p => p.Form > -3 && p.DisciplinePoints < 10)
            .ToList();

        var goalkeeper = availablePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Po)
            ?? CreateSparringPartner(PlayerPosition.Po);
        var defenders = FillWithSparring(
            availablePlayers.Where(p => p.Position == PlayerPosition.Di).ToList(),
            4,
            PlayerPosition.Di);
        var midfielders = FillWithSparring(
            availablePlayers.Where(p => p.Position == PlayerPosition.Ce).ToList(),
            3,
            PlayerPosition.Ce);
        var attackers = FillWithSparring(
            availablePlayers.Where(p => p.Position == PlayerPosition.At).ToList(),
            3,
            PlayerPosition.At);

        return new Formation
        {
            Goalkeeper = goalkeeper,
            Defenders = defenders,
            Midfielders = midfielders,
            Attackers = attackers
        };
    }

    private (Formation formation, FormationTactics tactics, (int def, int mid, int att) module)
        CreateRandomFormationAndTactics(Team team, (int def, int mid, int att)? excludedModule)
    {
        var preferredModule = GetPreferredModuleFromTactician(team);

        var moduleCandidates = AllowedModules
            .Where(m => excludedModule == null || m != excludedModule.Value)
            .ToList();

        if (moduleCandidates.Count == 0)
            moduleCandidates = [.. AllowedModules];

        var weightedModules = new List<(int def, int mid, int att)>();
        foreach (var module in moduleCandidates)
        {
            int weight = preferredModule.HasValue && module == preferredModule.Value ? 5 : 1;
            for (int i = 0; i < weight; i++)
            {
                weightedModules.Add(module);
            }
        }

        var shuffledModules = weightedModules
            .OrderBy(_ => _random.Next())
            .Distinct()
            .ToList();

        foreach (var module in shuffledModules)
        {
            var formation = TryBuildFormation(team, module);
            if (formation == null)
                continue;

            var tactics = CreateRandomTacticsForFormation(formation);
            return (formation, tactics, module);
        }

        var fallbackFormation = CreateDefaultFormation(team);
        return (fallbackFormation, new FormationTactics(), (4, 3, 3));
    }

    private (int def, int mid, int att)? GetPreferredModuleFromTactician(Team team)
    {
        var tactician = team.Staff.FirstOrDefault(s =>
            s.Type == BenchStaffType.Tactician &&
            !string.IsNullOrWhiteSpace(s.SpecializedTactic));

        if (tactician == null)
            return null;

        var tokens = tactician.SpecializedTactic!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
            return null;

        var moduleToken = tokens[0];
        var parts = moduleToken.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
            return null;

        if (!int.TryParse(parts[0], out int def) ||
            !int.TryParse(parts[1], out int mid) ||
            !int.TryParse(parts[2], out int att))
            return null;

        var preferred = (def, mid, att);
        return AllowedModules.Contains(preferred) ? preferred : null;
    }

    private Formation? TryBuildFormation(Team team, (int def, int mid, int att) module)
    {
        var availablePlayers = team.Players
            .Where(p => p.Form > -3 && p.DisciplinePoints < 10)
            .ToList();

        var goalkeeper = availablePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Po)
            ?? CreateSparringPartner(PlayerPosition.Po);

        var defendersPool = availablePlayers
            .Where(p => p.Position == PlayerPosition.Di)
            .OrderByDescending(p => p.Ability + p.Form)
            .ToList();
        defendersPool = FillWithSparring(defendersPool, module.def, PlayerPosition.Di);

        var midfielders = FillWithSparring(availablePlayers
            .Where(p => p.Position == PlayerPosition.Ce)
            .OrderByDescending(p => p.Ability + p.Form)
            .ToList(), module.mid, PlayerPosition.Ce);
        var attackers = FillWithSparring(availablePlayers
            .Where(p => p.Position == PlayerPosition.At)
            .OrderByDescending(p => p.Ability + p.Form)
            .ToList(), module.att, PlayerPosition.At);

        bool canUseLibero = module.def >= 3 && defendersPool.Count >= module.def;
        bool useLibero = canUseLibero && _random.Next(100) < 35;

        Player? libero = null;
        List<Player> defenders;

        if (useLibero)
        {
            libero = defendersPool.First();
            defenders = defendersPool.Skip(1).Take(module.def - 1).ToList();
        }
        else
        {
            defenders = defendersPool.Take(module.def).ToList();
        }

        return new Formation
        {
            Goalkeeper = goalkeeper,
            Libero = libero,
            Defenders = defenders,
            Midfielders = midfielders,
            Attackers = attackers
        };
    }

    private List<Player> FillWithSparring(List<Player> players, int required, PlayerPosition position)
    {
        var result = players.Take(required).ToList();
        while (result.Count < required)
        {
            result.Add(CreateSparringPartner(position));
        }

        return result;
    }

    private Player CreateSparringPartner(PlayerPosition position)
    {
        return new Player("Sparring Partner", position, 0, PlayerAge.Primavera)
        {
            Form = 0,
            Side = PlayerSide.SD,
            DisciplinePoints = 0
        };
    }

    private FormationTactics CreateRandomTacticsForFormation(Formation formation)
    {
        bool useLibero = formation.Libero != null;
        bool useOffsideTrap = !useLibero && _random.Next(100) < 35;
        bool useCatenaccio = _random.Next(100) < 15;

        var hardnessDistribution = CreateRandomHardnessDistribution(useLibero);
        int hardnessTotal = hardnessDistribution.Values.Sum();

        return new FormationTactics
        {
            HardnessTotal = hardnessTotal,
            HardnessDistribution = hardnessDistribution,
            UseOffsideTrap = useOffsideTrap,
            UseCatenaccio = useCatenaccio,
            CatenaccioPoints = useCatenaccio ? 7 : 0,
            CatenaccioDistribution = useCatenaccio
                ? new Dictionary<string, int>
                {
                    ["Li"] = useLibero ? 1 : 0,
                    ["Di"] = 3,
                    ["Ce"] = useLibero ? 3 : 4
                }
                : new Dictionary<string, int>()
        };
    }

    private void ApplyGreatPerformancePoints(Team team, Formation formation, FormationTactics tactics)
    {
        int maxUsable = Math.Min(3, Math.Max(0, team.GreatPerformancePoints));
        if (maxUsable == 0)
        {
            tactics.GreatPerformancePointsTotal = 0;
            tactics.GreatPerformanceDistribution = new Dictionary<string, int>
            {
                ["Po"] = 0,
                ["Li"] = 0,
                ["Di"] = 0,
                ["Ce"] = 0,
                ["At"] = 0
            };
            return;
        }

        int pointsToUse = _random.Next(0, maxUsable + 1);

        var distribution = new Dictionary<string, int>
        {
            ["Po"] = 0,
            ["Li"] = 0,
            ["Di"] = 0,
            ["Ce"] = 0,
            ["At"] = 0
        };

        bool hasLibero = formation.Libero != null;
        var allowedAreas = hasLibero
            ? new[] { "Po", "Li", "Di", "Ce", "At" }
            : new[] { "Po", "Di", "Ce", "At" };

        for (int i = 0; i < pointsToUse; i++)
        {
            var area = allowedAreas[_random.Next(allowedAreas.Length)];
            distribution[area]++;
        }

        tactics.GreatPerformancePointsTotal = pointsToUse;
        tactics.GreatPerformanceDistribution = distribution;

        if (pointsToUse > 0)
            team.GreatPerformancePoints -= pointsToUse;
    }

    private void ApplyTacticianBonus(Team team, Formation formation, FormationTactics tactics)
    {
        var tactician = team.Staff.FirstOrDefault(s =>
            s.Type == BenchStaffType.Tactician &&
            !string.IsNullOrWhiteSpace(s.SpecializedTactic));

        if (tactician == null)
            return;

        if (!TacticHelper.TacticMatches(tactician.SpecializedTactic!, formation, tactics))
            return;

        tactics.TacticianBonusPoints = 5;
        tactics.TacticianDistribution = new Dictionary<string, int>
        {
            ["Li"] = 0,
            ["Di"] = 0,
            ["Ce"] = 0,
            ["At"] = 0
        };

        var allowedAreas = formation.Libero != null
            ? new[] { "Li", "Di", "Ce", "At" }
            : new[] { "Di", "Ce", "At" };

        for (int i = 0; i < 5; i++)
        {
            var area = allowedAreas[_random.Next(allowedAreas.Length)];
            tactics.TacticianDistribution[area]++;
        }
    }

    private Dictionary<string, int> CreateRandomHardnessDistribution(bool hasLibero)
    {
        int totalHardness = _random.Next(0, 11);

        var distribution = new Dictionary<string, int>
        {
            ["Po"] = 0,
            ["Li"] = 0,
            ["Di"] = 0,
            ["Ce"] = 0,
            ["At"] = 0
        };

        var allowedAreas = hasLibero
            ? new[] { "Po", "Li", "Di", "Ce", "At" }
            : new[] { "Po", "Di", "Ce", "At" };

        for (int i = 0; i < totalHardness; i++)
        {
            var area = allowedAreas[_random.Next(allowedAreas.Length)];
            distribution[area]++;
        }

        return distribution;
    }

    private void ApplyHomeFieldAdvantage(FormationTactics tactics)
    {
        const int totalHomeFieldPoints = 7;

        tactics.HomeFieldAdvantagePoints = totalHomeFieldPoints;
        tactics.HomeFieldAdvantageDistribution = new Dictionary<string, int>
        {
            ["Di"] = 0,
            ["Ce"] = 0,
            ["At"] = 0
        };

        for (int i = 0; i < totalHomeFieldPoints; i++)
        {
            int areaRoll = _random.Next(3);
            string area = areaRoll switch
            {
                0 => "Di",
                1 => "Ce",
                _ => "At"
            };

            tactics.HomeFieldAdvantageDistribution[area]++;
        }
    }

    private void ManageSpecialPoints()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow(" GESTIONE PUNTI SPECIALI (PS) "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        if (_allTeams.Count == 0)
        {
            Console.WriteLine(CreateTableRow(" Nessuna squadra inserita. Usa l'opzione 1 per inserire i dati. "));
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        // Mostra squadre con PS disponibili
        Console.WriteLine(CreateTableRow(" N.  Squadra                          PA  PGP   M   PS "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        for (int i = 0; i < _allTeams.Count; i++)
        {
            var team = _allTeams[i];
            string content = $" {(i + 1),2}. {team.Name,-30} {team.TrainingPoints,2}  {team.GreatPerformancePoints,3}  {team.Money,3}  {team.SpecialPoints,3} ";
            Console.WriteLine(CreateTableRow(content));
        }

        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");

        Console.Write("\nSeleziona squadra (numero, 0 per annullare): ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int teamIndex) || teamIndex == 0 || teamIndex > _allTeams.Count)
        {
            return;
        }

        var selectedTeam = _allTeams[teamIndex - 1];

        Console.WriteLine($"\nSquadra selezionata: {selectedTeam.Name}");
        Console.WriteLine($"PS disponibili: {selectedTeam.SpecialPoints}");

        if (selectedTeam.SpecialPoints == 0)
        {
            Console.WriteLine("\nNessun Punto Speciale disponibile per questa squadra.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        // Menu conversione
        Console.WriteLine("\n--- CONVERSIONE PUNTI SPECIALI ---");
        Console.WriteLine("1. Converti PS in PA (Punti Allenamento) [1 PS = 1 PA]");
        Console.WriteLine("2. Converti PS in PGP (Punti Grande Prestazione) [1 PS = 1 PGP]");
        Console.WriteLine("3. Converti PS in M (Milioni) [1 PS = 20 M]");
        Console.WriteLine("0. Annulla");
        Console.Write("\nScegli tipo di conversione: ");

        string? conversionChoice = Console.ReadLine();

        if (conversionChoice == "0" || string.IsNullOrWhiteSpace(conversionChoice))
        {
            return;
        }

        Console.Write($"\nQuantità di PS da convertire (max {selectedTeam.SpecialPoints}): ");
        string? amountInput = Console.ReadLine();
        if (!int.TryParse(amountInput, out int amount) || amount <= 0)
        {
            Console.WriteLine("\nQuantità non valida!");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        bool success = false;
        string conversionType = "";
        string result = "";

        switch (conversionChoice)
        {
            case "1":
                success = selectedTeam.ConvertSpecialPointsToTraining(amount);
                conversionType = "PA";
                result = $"+{amount} PA";
                break;
            case "2":
                success = selectedTeam.ConvertSpecialPointsToGreatPerformance(amount);
                conversionType = "PGP";
                result = $"+{amount} PGP";
                break;
            case "3":
                success = selectedTeam.ConvertSpecialPointsToMoney(amount);
                conversionType = "M";
                result = $"+{amount * 20} M";
                break;
            default:
                Console.WriteLine("\nOpzione non valida!");
                Console.WriteLine("\nPremi un tasto per continuare...");
                Console.ReadKey();
                return;
        }

        if (success)
        {
            Console.WriteLine($"\n[OK] Conversione completata!");
            Console.WriteLine($"     - {amount} PS convertiti in {result}");
            Console.WriteLine($"\nRisorse aggiornate per {selectedTeam.Name}:");
            Console.WriteLine($"  PA:  {selectedTeam.TrainingPoints}");
            Console.WriteLine($"  PGP: {selectedTeam.GreatPerformancePoints}");
            Console.WriteLine($"  M:   {selectedTeam.Money}");
            Console.WriteLine($"  PS:  {selectedTeam.SpecialPoints}");
        }
        else
        {
            Console.WriteLine($"\n[ERRORE] Conversione fallita: PS insufficienti!");
        }

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void GenerateRandomTeams()
    {
        Console.Clear();
        Console.Write("Numero di squadre da generare (default 12): ");
        string? input = Console.ReadLine();
        int teamCount = string.IsNullOrWhiteSpace(input) ? 12 : int.Parse(input);

        Console.WriteLine("\n[!] ATTENZIONE: Questa operazione sostituirà tutte le squadre esistenti.");
        Console.Write("Continuare? (S/N, default S): ");
        string? confirm = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(confirm) && confirm.ToUpper() != "S")
        {
            Console.WriteLine("\nOperazione annullata.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        _allTeams.Clear();
        _generatedPlayerNames.Clear();
        _currentSeason = null; // Reset stagione

        var random = new Random();
        Console.WriteLine($"\nGenerazione di {teamCount} squadre casuali...\n");

        for (int i = 1; i <= teamCount; i++)
        {
            var team = _teamFactory.CreateRandomTeam(i, random, _generatedPlayerNames);
            _allTeams.Add(team);
            Console.WriteLine($"[OK] {i,2}/{teamCount} - {team.Name} (Manager: {team.ManagerName})");
        }

        Console.WriteLine($"\n[COMPLETATO] {_allTeams.Count} squadre generate con successo!");
        Console.WriteLine("\nDettagli:");
        Console.WriteLine($"  - Ogni squadra ha giocatori adulti con 70 punti abilita' distribuiti");
        Console.WriteLine($"  - Eta' I: 34 punti, Eta' II/III/IV: 12 punti ciascuna");
        Console.WriteLine($"  - 3 Juniores Ab 5 Fo +2, 3 Juniores Ab 3 Fo +2, 6 Primavera Ab 2 Fo +2");
        Console.WriteLine($"  - Staff e risorse (PA, PGP, M, PS) assegnati casualmente");

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void SaveTeamsToJson()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════��═══════════════╗");
        Console.WriteLine(CreateTableRow(" SALVA SQUADRE IN JSON "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        if (_allTeams.Count == 0)
        {
            Console.WriteLine(CreateTableRow(" Nessuna squadra da salvare. Usa l'opzione 1 o 8 per creare squadre. "));
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        Console.Write("\nNome file (default: teams.json): ");
        string? filename = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(filename))
        {
            filename = "teams.json";
        }

        if (!filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            filename += ".json";
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string jsonString = JsonSerializer.Serialize(_allTeams, options);
            File.WriteAllText(filename, jsonString);

            Console.WriteLine($"\n[OK] Salvataggio completato!");
            Console.WriteLine($"     File: {Path.GetFullPath(filename)}");
            Console.WriteLine($"     Squadre salvate: {_allTeams.Count}");
            Console.WriteLine($"     Dimensione: {new FileInfo(filename).Length / 1024} KB");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERRORE] Impossibile salvare il file: {ex.Message}");
        }

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void LoadTeamsFromJson()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow(" CARICA SQUADRE DA JSON "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        Console.Write("\nNome file (default: teams.json): ");
        string? filename = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(filename))
        {
            filename = "teams.json";
        }

        if (!filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            filename += ".json";
        }

        if (!File.Exists(filename))
        {
            Console.WriteLine($"\n[ERRORE] File non trovato: {filename}");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\n[!] ATTENZIONE: Questa operazione sostituira' tutte le squadre esistenti.");
        Console.Write("Continuare? (S/N, default S): ");
        string? confirm = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(confirm) && confirm.ToUpper() != "S")
        {
            Console.WriteLine("\nOperazione annullata.");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        try
        {
            string jsonString = File.ReadAllText(filename);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var loadedTeams = JsonSerializer.Deserialize<List<Team>>(jsonString, options);

            if (loadedTeams == null || loadedTeams.Count == 0)
            {
                Console.WriteLine("\n[ERRORE] Il file non contiene squadre valide.");
                Console.WriteLine("\nPremi un tasto per continuare...");
                Console.ReadKey();
                return;
            }

            _allTeams = loadedTeams;
            _currentSeason = null; // Reset stagione corrente

            Console.WriteLine($"\n[OK] Caricamento completato!");
            Console.WriteLine($"     File: {Path.GetFullPath(filename)}");
            Console.WriteLine($"     Squadre caricate: {_allTeams.Count}");
            Console.WriteLine("\nRiepilogo squadre caricate:");

            for (int i = 0; i < _allTeams.Count && i < 10; i++)
            {
                var team = _allTeams[i];
                Console.WriteLine($"  {i + 1,2}. {team.Name,-25} (Manager: {team.ManagerName})");
            }

            if (_allTeams.Count > 10)
            {
                Console.WriteLine($"  ... e altre {_allTeams.Count - 10} squadre");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"\n[ERRORE] File JSON non valido: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERRORE] Impossibile caricare il file: {ex.Message}");
        }

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void DisplayTeamDetails()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow(" DETTAGLI SQUADRA "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        if (_allTeams.Count == 0)
        {
            Console.WriteLine(CreateTableRow(" Nessuna squadra disponibile. Usa l'opzione 1 o 8 per creare squadre. "));
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        // Mostra lista squadre
        Console.WriteLine(CreateTableRow(" N.  Squadra              Manager "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        for (int i = 0; i < _allTeams.Count; i++)
        {
            var team = _allTeams[i];
            string content = $" {(i + 1),2}. {team.Name,-20} {team.ManagerName,-35} ";
            Console.WriteLine(CreateTableRow(content));
        }

        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");

        Console.Write("\nSeleziona squadra (numero, 0 per annullare): ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int teamIndex) || teamIndex == 0 || teamIndex > _allTeams.Count)
        {
            return;
        }

        var selectedTeam = _allTeams[teamIndex - 1];

        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow($" {selectedTeam.Name} "));
        Console.WriteLine(CreateTableRow($" Manager: {selectedTeam.ManagerName} "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine(CreateTableRow($" Risorse: PA={selectedTeam.TrainingPoints,3}  PGP={selectedTeam.GreatPerformancePoints,3}  M={selectedTeam.Money,4}  PS={selectedTeam.SpecialPoints,3} "));
        Console.WriteLine(CreateTableRow(" GIOCATORI "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine(CreateTableRow(" Nome                 Ruolo Eta'    Ab Lato Fo  PD  Status "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        // Ordina per ruolo e poi per abilità
        var sortedPlayers = selectedTeam.Players
            .OrderBy(p => p.Position)
            .ThenByDescending(p => p.Ability)
            .ToList();

        foreach (var player in sortedPlayers)
        {
            string pos = player.Position.ToString();
            string age = player.Age.ToString();
            string side = player.Side.ToString();
            string form = player.Form >= 0 ? $"+{player.Form}" : player.Form.ToString();
            string disciplinePoints = player.DisciplinePoints.ToString();

            string status = "";
            if (player.Form <= -3)
                status = "[INF]";
            else if (player.DisciplinePoints >= 8)
                status = "[!PD]";

            string content = $" {player.Name,-20} {pos,-5} {age,-9} {player.Ability,2} {side,-4} {form,3} {disciplinePoints,3}  {status,-18} ";
            Console.WriteLine(CreateTableRow(content));
        }

        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine(CreateTableRow(" STAFF "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine(CreateTableRow(" Ruolo                Nome "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        if (selectedTeam.Staff.Count == 0)
        {
            Console.WriteLine(CreateTableRow(" Nessuno staff presente "));
        }
        else
        {
            // Ordina per tipo di staff
            var sortedStaff = selectedTeam.Staff.OrderBy(s => s.Type).ToList();

            foreach (var staff in sortedStaff)
            {
                string role = staff.Type switch
                {
                    BenchStaffType.Coach => "Allenatore",
                    BenchStaffType.Masseur => "Massaggiatore",
                    BenchStaffType.Scout => "Scout",
                    BenchStaffType.Tactician => "Tattico",
                    _ => "Sconosciuto"
                };

                string staffDisplayName = staff.Type == BenchStaffType.Tactician && !string.IsNullOrWhiteSpace(staff.SpecializedTactic)
                    ? $"{staff.Name} ({staff.SpecializedTactic})"
                    : staff.Name;

                string content = $" {role,-20} {staffDisplayName,-49} ";
                Console.WriteLine(CreateTableRow(content));
            }
        }

        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine("\nLegenda:");
        Console.WriteLine("  Ruolo: Po=Portiere, Di=Difensore, Ce=Centrocampista, At=Attaccante");
        Console.WriteLine("  Eta': I-IV=Adulti, Juniores, Primavera");
        Console.WriteLine("  Ab=Abilita', Lato: D=Destro, S=Sinistro, SD=Ambidestro");
        Console.WriteLine("  Fo=Forma, PD=Punti Disciplina");
        Console.WriteLine("  Status: [INF]=Infortunato (Fo <= -3), [!PD]=Rischio ammonizione (PD >= 8)");
        Console.WriteLine($"\nTotale giocatori: {selectedTeam.Players.Count}");
        Console.WriteLine($"Totale staff: {selectedTeam.Staff.Count}");

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private void DisplayCalendar()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow(" CALENDARIO COMPETIZIONI "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        if (_currentSeason == null)
        {
            Console.WriteLine(CreateTableRow(" Nessuna stagione attiva. Usa l'opzione 3 per creare una stagione. "));
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        if (_currentSeason.Competitions.Count == 0)
        {
            Console.WriteLine(CreateTableRow(" Nessuna competizione trovata. "));
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        // Mostra elenco competizioni disponibili
        Console.WriteLine(CreateTableRow(" COMPETIZIONI DISPONIBILI "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        for (int i = 0; i < _currentSeason.Competitions.Count; i++)
        {
            var comp = _currentSeason.Competitions[i];
            string status = comp.IsCompleted ? "[COMPLETATA]" : $"[Giornata {comp.CurrentRound}]";
            string content = $" {(i + 1),2}. {comp.Name,-35} {status,-20} ";
            Console.WriteLine(CreateTableRow(content));
        }

        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine(CreateTableRow(" 0. Torna al menu principale "));
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");

        Console.Write("\nSeleziona competizione (numero): ");
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int compIndex) || compIndex == 0 || compIndex > _currentSeason.Competitions.Count)
        {
            return;
        }

        var selectedComp = _currentSeason.Competitions[compIndex - 1];
        DisplayCompetitionCalendar(selectedComp);
    }

    private void DisplayCompetitionCalendar(Competition competition)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow($" CALENDARIO: {competition.Name.ToUpper()} "));
        Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

        if (competition.Matches.Count == 0)
        {
            Console.WriteLine(CreateTableRow(" Nessuna partita programmata. "));
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nPremi un tasto per continuare...");
            Console.ReadKey();
            return;
        }

        // Raggruppa le partite per giornata/turno
        var matchesByRound = competition.Matches
            .GroupBy(m => m.Round)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var roundGroup in matchesByRound)
        {
            int round = roundGroup.Key;
            string roundTitle = competition.Type == CompetitionType.Championship 
                ? $"GIORNATA {round}" 
                : $"TURNO {round}";

            Console.WriteLine(CreateTableRow($" {roundTitle} "));
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");

            foreach (var match in roundGroup.OrderBy(m => m.HomeTeam.Name))
            {
                string result;
                if (match.IsPlayed)
                {
                    result = $"{match.HomeTeam.Name,-25} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam.Name,-25}";
                }
                else
                {
                    result = $"{match.HomeTeam.Name,-25} vs {match.AwayTeam.Name,-25}";
                }

                string status = match.IsPlayed ? "[GIOCATA]" : "[DA GIOCARE]";
                string content = $" {result} {status,-12} ";
                Console.WriteLine(CreateTableRow(content));
            }

            Console.WriteLine("╠════════════════════════════════════════════════════════════════════════╣");
        }

        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");

        Console.WriteLine($"\nTotale giornate/turni: {matchesByRound.Count}");
        Console.WriteLine($"Partite totali: {competition.Matches.Count}");
        Console.WriteLine($"Partite giocate: {competition.Matches.Count(m => m.IsPlayed)}");
        Console.WriteLine($"Partite da giocare: {competition.Matches.Count(m => !m.IsPlayed)}");

        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }
}
