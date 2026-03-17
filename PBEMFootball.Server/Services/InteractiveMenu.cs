namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

public class InteractiveMenu
{
    private readonly SeasonManager _seasonManager;
    private readonly TeamFactory _teamFactory;
    private Season? _currentSeason;
    private List<Team> _allTeams = new();

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
            // Crea formazioni di default (semplificato per demo)
            var homeFormation = CreateDefaultFormation(match.HomeTeam);
            var awayFormation = CreateDefaultFormation(match.AwayTeam);

            match.HomeFormation = homeFormation;
            match.AwayFormation = awayFormation;

            matchEngine.SimulateMatch(match);

            Console.WriteLine($"{match.HomeTeam.Name,-20} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam.Name,-20}");

            // Aggiorna classifica
            _seasonManager.UpdateStandings(championship, match);
        }

        championship.CurrentRound = nextRound;

        Console.WriteLine($"\n✓ Giornata {nextRound} completata!");
        Console.WriteLine("\nPremi un tasto per continuare...");
        Console.ReadKey();
    }

    private Formation CreateDefaultFormation(Team team)
    {
        return new Formation
        {
            Goalkeeper = team.Players.First(p => p.Position == PlayerPosition.Po),
            Defenders = team.Players.Where(p => p.Position == PlayerPosition.Di).Take(4).ToList(),
            Midfielders = team.Players.Where(p => p.Position == PlayerPosition.Ce).Take(3).ToList(),
            Attackers = team.Players.Where(p => p.Position == PlayerPosition.At).Take(3).ToList()
        };
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
        _currentSeason = null; // Reset stagione

        var random = new Random();
        Console.WriteLine($"\nGenerazione di {teamCount} squadre casuali...\n");

        for (int i = 1; i <= teamCount; i++)
        {
            var team = _teamFactory.CreateRandomTeam(i, random);
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

                string content = $" {role,-20} {staff.Name,-49} ";
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
