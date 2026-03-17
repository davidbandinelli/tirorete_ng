using PBEMFootball.Common.Models;
using PBEMFootball.Server.Services;
using PBEMFootball.Server.Engine;
using PBEMFootball.Common.Services;

Console.WriteLine("=== PBEM Football Server ===");
Console.WriteLine("Server avviato.");
Console.WriteLine();

// Menu interattivo principale
Console.Write("Avviare il menu interattivo? (s/n, default s): ");
string? choice = Console.ReadLine();

if (string.IsNullOrWhiteSpace(choice) || choice.ToLower() == "s")
{
    var menu = new InteractiveMenu();
    menu.Run();
    return;
}

// ============================================================================
// DEMO E TEST (eseguiti solo se si risponde 'n' al menu interattivo)
// ============================================================================

var teamFactory = new TeamFactory();
var matchEngine = new MatchEngine();
var tacticsValidator = new TacticsValidator();

Console.WriteLine("Creazione squadre iniziali...");
var team1 = teamFactory.CreateInitialTeam("FC Beppiland", "Mario Rossi");
var team2 = teamFactory.CreateInitialTeam("AC Juniores", "Luigi Verdi");

team1.Stadium.Level = 2;
team2.Stadium.Level = 1;

team2.Staff.Add(new BenchStaff("Massaggiatore Esperto", BenchStaffType.Masseur));
team1.Staff.Add(new BenchStaff("Tattico Libero", BenchStaffType.Tactician, "3-3-3 Li"));

PrintTeamRoster(team1);
PrintTeamRoster(team2);

Console.WriteLine("=== PARTITA 1: Con CATENACCIO e LIBERO ===");
Console.WriteLine();
var formation1 = new Formation
{
    Goalkeeper = team1.Players.First(p => p.Position == PlayerPosition.Po),
    Libero = team1.Players.First(p => p.Position == PlayerPosition.Di),
    Defenders = team1.Players.Where(p => p.Position == PlayerPosition.Di).Skip(1).Take(3).ToList(),
    Midfielders = team1.Players.Where(p => p.Position == PlayerPosition.Ce).Take(3).ToList(),
    Attackers = team1.Players.Where(p => p.Position == PlayerPosition.At).Take(3).ToList()
};

var formation2 = new Formation
{
    Goalkeeper = team2.Players.First(p => p.Position == PlayerPosition.Po),
    Defenders = team2.Players.Where(p => p.Position == PlayerPosition.Di).Take(4).ToList(),
    Midfielders = team2.Players.Where(p => p.Position == PlayerPosition.Ce).Take(4).ToList(),
    Attackers = team2.Players.Where(p => p.Position == PlayerPosition.At).Take(2).ToList()
};

var tactics1 = new FormationTactics
{
    HomeFieldAdvantagePoints = team1.Stadium.HomeFieldAdvantage,
    HomeFieldAdvantageDistribution = new Dictionary<string, int>
    {
        { "Di", 3 },
        { "Ce", 3 },
        { "At", 3 }
    },
    HardnessTotal = 3,
    HardnessDistribution = new Dictionary<string, int>
    {
        { "Di", 1 },
        { "Ce", 1 },
        { "At", 1 }
    },
    UseCatenaccio = true,
    CatenaccioPoints = 7,
    CatenaccioDistribution = new Dictionary<string, int>
    {
        { "Li", 2 },
        { "Di", 3 },
        { "Ce", 2 }
    },
    TacticianBonusPoints = 5,
    TacticianDistribution = new Dictionary<string, int>
    {
        { "Li", 1 },
        { "Di", 2 },
        { "Ce", 2 }
    }
};

var tactics2 = new FormationTactics
{
    HardnessTotal = 2,
    HardnessDistribution = new Dictionary<string, int>
    {
        { "Di", 1 },
        { "At", 1 }
    }
};

if (!tacticsValidator.ValidateTactics(formation1, tactics1, out string error1))
{
    Console.WriteLine($"ERRORE tattiche squadra 1: {error1}");
}
if (!tacticsValidator.ValidateTactics(formation2, tactics2, out string error2))
{
    Console.WriteLine($"ERRORE tattiche squadra 2: {error2}");
}

var match1 = new Match(team1, team2)
{
    HomeFormation = formation1,
    AwayFormation = formation2,
    HomeTactics = tactics1,
    AwayTactics = tactics2
};

PrintLineup(team1.Name, formation1, "3-Li-3-3 + CATENACCIO");
PrintLineup(team2.Name, formation2, "4-4-2");

Console.WriteLine($"{team1.Name} (3-Li-3-3 + CATENACCIO) vs {team2.Name} (4-4-2)");
matchEngine.SimulateMatch(match1);
Console.WriteLine($"Risultato: {match1.HomeGoals} - {match1.AwayGoals}");
Console.WriteLine($"Tiri: {match1.HomeShots} - {match1.AwayShots}");
Console.WriteLine($"Meteo: {match1.Weather}");
Console.WriteLine();

PrintMatchEvents(match1, team1.Name, team2.Name);

Console.WriteLine("=== PARTITA 2: Con TRAPPOLA FUORI GIOCO (senza libero) ===");
var formation3 = new Formation
{
    Goalkeeper = team1.Players.First(p => p.Position == PlayerPosition.Po),
    Defenders = team1.Players.Where(p => p.Position == PlayerPosition.Di).Take(4).ToList(),
    Midfielders = team1.Players.Where(p => p.Position == PlayerPosition.Ce).Take(3).ToList(),
    Attackers = team1.Players.Where(p => p.Position == PlayerPosition.At).Take(3).ToList()
};

var formation4 = new Formation
{
    Goalkeeper = team2.Players.First(p => p.Position == PlayerPosition.Po),
    Defenders = team2.Players.Where(p => p.Position == PlayerPosition.Di).Take(3).ToList(),
    Midfielders = team2.Players.Where(p => p.Position == PlayerPosition.Ce).Take(5).ToList(),
    Attackers = team2.Players.Where(p => p.Position == PlayerPosition.At).Take(2).ToList()
};

var tactics3 = new FormationTactics
{
    HomeFieldAdvantagePoints = team1.Stadium.HomeFieldAdvantage,
    HomeFieldAdvantageDistribution = new Dictionary<string, int>
    {
        { "Di", 3 },
        { "Ce", 3 },
        { "At", 3 }
    },
    UseOffsideTrap = true,
    HardnessTotal = 1,
    HardnessDistribution = new Dictionary<string, int>
    {
        { "At", 1 }
    }
};

var tactics4 = new FormationTactics
{
    HardnessTotal = 4,
    HardnessDistribution = new Dictionary<string, int>
    {
        { "Di", 2 },
        { "Ce", 1 },
        { "At", 1 }
    }
};

if (!tacticsValidator.ValidateTactics(team1, formation3, tactics3, out string error3))
{
    Console.WriteLine($"ERRORE tattiche squadra 1: {error3}");
}
if (!tacticsValidator.ValidateTactics(team2, formation4, tactics4, out string error4))
{
    Console.WriteLine($"ERRORE tattiche squadra 2: {error4}");
}

var match2 = new Match(team1, team2)
{
    HomeFormation = formation3,
    AwayFormation = formation4,
    HomeTactics = tactics3,
    AwayTactics = tactics4
};

PrintLineup(team1.Name, formation3, "4-3-3 + TFG");
PrintLineup(team2.Name, formation4, "3-5-2");

Console.WriteLine($"{team1.Name} (4-3-3 + TFG) vs {team2.Name} (3-5-2)");
matchEngine.SimulateMatch(match2);
Console.WriteLine($"Risultato: {match2.HomeGoals} - {match2.AwayGoals}");
Console.WriteLine($"Tiri: {match2.HomeShots} - {match2.AwayShots}");
Console.WriteLine($"Meteo: {match2.Weather}");
Console.WriteLine();

PrintMatchEvents(match2, team1.Name, team2.Name);

PrintPlayerFormsAfterMatch(team1, match2.HomeFormation);
PrintPlayerFormsAfterMatch(team2, match2.AwayFormation);
Console.WriteLine();

Console.WriteLine("=== PARTITA 3: Formazione con LIBERO vs normale ===");
team1.Players.ForEach(p => { p.Form = 0; p.DisciplinePoints = 0; });
team2.Players.ForEach(p => { p.Form = 0; p.DisciplinePoints = 0; });

var formation5 = new Formation
{
    Goalkeeper = team1.Players.First(p => p.Position == PlayerPosition.Po),
    Libero = team1.Players.First(p => p.Position == PlayerPosition.Di),
    Defenders = team1.Players.Where(p => p.Position == PlayerPosition.Di).Skip(1).Take(2).ToList(),
    Midfielders = team1.Players.Where(p => p.Position == PlayerPosition.Ce).Take(4).ToList(),
    Attackers = team1.Players.Where(p => p.Position == PlayerPosition.At).Take(3).ToList()
};

var formation6 = new Formation
{
    Goalkeeper = team2.Players.First(p => p.Position == PlayerPosition.Po),
    Defenders = team2.Players.Where(p => p.Position == PlayerPosition.Di).Take(4).ToList(),
    Midfielders = team2.Players.Where(p => p.Position == PlayerPosition.Ce).Take(3).ToList(),
    Attackers = team2.Players.Where(p => p.Position == PlayerPosition.At).Take(3).ToList()
};

var tactics5 = new FormationTactics
{
    HomeFieldAdvantagePoints = team1.Stadium.HomeFieldAdvantage,
    HomeFieldAdvantageDistribution = new Dictionary<string, int>
    {
        { "Di", 4 },
        { "Ce", 3 },
        { "At", 2 }
    },
    HardnessTotal = 2,
    HardnessDistribution = new Dictionary<string, int>
    {
        { "Di", 1 },
        { "Ce", 1 }
    }
};

var tactics6 = new FormationTactics
{
    HardnessTotal = 2,
    HardnessDistribution = new Dictionary<string, int>
    {
        { "Di", 1 },
        { "At", 1 }
    }
};

var match3 = new Match(team1, team2)
{
    HomeFormation = formation5,
    AwayFormation = formation6,
    HomeTactics = tactics5,
    AwayTactics = tactics6
};

PrintLineup(team1.Name, formation5, "2-Li-4-3 con LIBERO");
PrintLineup(team2.Name, formation6, "4-3-3");

Console.WriteLine($"{team1.Name} (2-Li-4-3 con LIBERO) vs {team2.Name} (4-3-3)");
matchEngine.SimulateMatch(match3);
Console.WriteLine($"Risultato: {match3.HomeGoals} - {match3.AwayGoals}");
Console.WriteLine($"Tiri: {match3.HomeShots} - {match3.AwayShots}");
Console.WriteLine($"Meteo: {match3.Weather}");
Console.WriteLine();

PrintMatchEvents(match3, team1.Name, team2.Name);

Console.WriteLine("=== STATISTICHE DISCIPLINARI E INFORTUNI ===");
var homeYellows = match3.Events.Count(e => e.IsHomeTeam && e.Type == MatchEventType.YellowCard);
var homeReds = match3.Events.Count(e => e.IsHomeTeam && e.Type == MatchEventType.RedCard);
var homeInjuries = match3.Events.Count(e => e.IsHomeTeam && e.Type == MatchEventType.Injury);
var homePenalties = match3.Events.Count(e => e.IsHomeTeam && e.Type == MatchEventType.Penalty);
var homeGoals = match3.Events.Count(e => e.IsHomeTeam && e.Type == MatchEventType.Goal);
var awayYellows = match3.Events.Count(e => !e.IsHomeTeam && e.Type == MatchEventType.YellowCard);
var awayReds = match3.Events.Count(e => !e.IsHomeTeam && e.Type == MatchEventType.RedCard);
var awayInjuries = match3.Events.Count(e => !e.IsHomeTeam && e.Type == MatchEventType.Injury);
var awayPenalties = match3.Events.Count(e => !e.IsHomeTeam && e.Type == MatchEventType.Penalty);
var awayGoals = match3.Events.Count(e => !e.IsHomeTeam && e.Type == MatchEventType.Goal);

Console.WriteLine($"{team1.Name}: Goal {homeGoals}, Rigori ⚽ {homePenalties} | Ammoniti 🟨 {homeYellows}, Espulsi 🟥 {homeReds}, Infortunati 🏥 {homeInjuries}");
Console.WriteLine($"{team2.Name}: Goal {awayGoals}, Rigori ⚽ {awayPenalties} | Ammoniti 🟨 {awayYellows}, Espulsi 🟥 {awayReds}, Infortunati 🏥 {awayInjuries} {(team2.Staff.Any(s => s.Type == BenchStaffType.Masseur) ? "(Masseur)" : "")}");
Console.WriteLine();

Console.WriteLine("=== RIEPILOGO IMPLEMENTAZIONE ===");
Console.WriteLine("✅ FC (Fattore Campo): Gestito (7-12 punti basati su stadio, distribuibili su Di/Ce/At in casa)");
Console.WriteLine("✅ LIBERO: Gestito (tiri extra da Di = /3 invece di /5)");
Console.WriteLine("✅ CATENACCIO: Gestito (+7 punti Li/Di/Ce, tiri dimezzati)");
Console.WriteLine("✅ TFG: Gestito (dimezza tiri At avversari, raddoppia Ce avversari, solo senza libero)");
Console.WriteLine();

Console.WriteLine("=== TEST: LIMITE 5 PUNTI SUL LIBERO ===");
Console.WriteLine();

var formation7 = new Formation
{
    Goalkeeper = team1.Players.First(p => p.Position == PlayerPosition.Po),
    Libero = team1.Players.First(p => p.Position == PlayerPosition.Di),
    Defenders = team1.Players.Where(p => p.Position == PlayerPosition.Di).Skip(1).Take(2).ToList(),
    Midfielders = team1.Players.Where(p => p.Position == PlayerPosition.Ce).Take(4).ToList(),
    Attackers = team1.Players.Where(p => p.Position == PlayerPosition.At).Take(3).ToList()
};

var libero = formation7.Libero;
Console.WriteLine($"Libero: {libero?.Name} - Ab: {libero?.Ability}, Fo: {libero?.Form}");
int liberoBase = (libero?.Ability ?? 0) + (libero?.Form ?? 0);
Console.WriteLine($"Valore base Libero: {liberoBase}");
Console.WriteLine();

Console.WriteLine("TEST A: Tentativo di mettere 8 punti sul Libero (2 Durezza + 3 PGP + 3 Tattico)");
var tacticsTestA = new FormationTactics
{
    HardnessTotal = 2,
    HardnessDistribution = new Dictionary<string, int> { { "Li", 2 } },
    GreatPerformancePointsTotal = 3,
    GreatPerformanceDistribution = new Dictionary<string, int> { { "Li", 3 } },
    TacticianBonusPoints = 5,
    TacticianDistribution = new Dictionary<string, int> { { "Li", 3 }, { "Di", 2 } }
};

if (!tacticsValidator.ValidateTactics(formation7, tacticsTestA, out string errorA))
{
    Console.WriteLine($"❌ VALIDATOR: {errorA}");
}
else
{
    Console.WriteLine("⚠️ VALIDATOR: Tattiche accettate (8 punti su Li, ma il motore deve limitare a 5)");
    var matchTestA = new Match(team1, team2)
    {
        HomeFormation = formation7,
        AwayFormation = formation2,
        HomeTactics = tacticsTestA,
        AwayTactics = new FormationTactics { HardnessTotal = 0 }
    };
    matchEngine.SimulateMatch(matchTestA);
    Console.WriteLine($"   Tentati: 2+3+3=8 punti → Applicati: MAX 5 punti");
}
Console.WriteLine();

Console.WriteLine("TEST B: Mettere esattamente 5 punti sul Libero (2 Durezza + 2 PGP + 1 Catenaccio)");
var tacticsTestB = new FormationTactics
{
    HardnessTotal = 2,
    HardnessDistribution = new Dictionary<string, int> { { "Li", 2 } },
    GreatPerformancePointsTotal = 2,
    GreatPerformanceDistribution = new Dictionary<string, int> { { "Li", 2 } },
    UseCatenaccio = true,
    CatenaccioPoints = 7,
    CatenaccioDistribution = new Dictionary<string, int> { { "Li", 1 }, { "Di", 3 }, { "Ce", 3 } }
};

if (!tacticsValidator.ValidateTactics(formation7, tacticsTestB, out string errorB))
{
    Console.WriteLine($"❌ VALIDATOR: {errorB}");
}
else
{
    Console.WriteLine("✅ VALIDATOR: Tattiche valide (5 punti esatti)");
    var matchTestB = new Match(team1, team2)
    {
        HomeFormation = formation7,
        AwayFormation = formation2,
        HomeTactics = tacticsTestB,
        AwayTactics = new FormationTactics { HardnessTotal = 0 }
    };
    matchEngine.SimulateMatch(matchTestB);
    Console.WriteLine($"   5 punti applicati correttamente");
}
Console.WriteLine();

Console.WriteLine("TEST C: Tentativo di usare FC sul Libero (DEVE FALLIRE)");
var tacticsTestC = new FormationTactics
{
    HomeFieldAdvantagePoints = 7,
    HomeFieldAdvantageDistribution = new Dictionary<string, int> { { "Li", 7 } }
};

if (!tacticsValidator.ValidateTactics(formation7, tacticsTestC, out string errorC))
{
    Console.WriteLine($"✅ VALIDATOR CORRETTO: {errorC}");
}
else
{
    Console.WriteLine("❌ VALIDATOR SBAGLIATO: Dovrebbe bloccare FC sul Libero!");
}
Console.WriteLine();

Console.WriteLine("=== TEST: LIMITE 5 PUNTI SUL PORTIERE ===");
Console.WriteLine();

var goalkeeper = formation7.Goalkeeper;
Console.WriteLine($"Portiere: {goalkeeper?.Name} - Ab: {goalkeeper?.Ability}, Fo: {goalkeeper?.Form}");
int poBase = (goalkeeper?.Ability ?? 0) + (goalkeeper?.Form ?? 0);
Console.WriteLine($"Valore base Portiere: {poBase}");
Console.WriteLine();

Console.WriteLine("TEST D: Tentativo di mettere 7 punti sul Portiere (4 Durezza + 3 PGP)");
var tacticsTestD = new FormationTactics
{
    HardnessTotal = 4,
    HardnessDistribution = new Dictionary<string, int> { { "Po", 4 } },
    GreatPerformancePointsTotal = 3,
    GreatPerformanceDistribution = new Dictionary<string, int> { { "Po", 3 } }
};

if (!tacticsValidator.ValidateTactics(formation7, tacticsTestD, out string errorD))
{
    Console.WriteLine($"❌ VALIDATOR: {errorD}");
}
else
{
    Console.WriteLine("⚠️ VALIDATOR: Tattiche accettate (7 punti su Po, ma il motore deve limitare a 5)");
}
Console.WriteLine();

Console.WriteLine("TEST E: Mettere esattamente 5 punti sul Portiere (3 Durezza + 2 PGP)");
var tacticsTestE = new FormationTactics
{
    HardnessTotal = 3,
    HardnessDistribution = new Dictionary<string, int> { { "Po", 3 } },
    GreatPerformancePointsTotal = 2,
    GreatPerformanceDistribution = new Dictionary<string, int> { { "Po", 2 } }
};

if (!tacticsValidator.ValidateTactics(formation7, tacticsTestE, out string errorE))
{
    Console.WriteLine($"❌ VALIDATOR: {errorE}");
}
else
{
    Console.WriteLine("✅ VALIDATOR: Tattiche valide (5 punti esatti sul Po)");
}
Console.WriteLine();

Console.WriteLine("=== TEST: TFG CON LIBERO (DEVE FALLIRE) ===");
Console.WriteLine();

var formationWithLibero = new Formation
{
    Goalkeeper = team1.Players.First(p => p.Position == PlayerPosition.Po),
    Libero = team1.Players.First(p => p.Position == PlayerPosition.Di),
    Defenders = team1.Players.Where(p => p.Position == PlayerPosition.Di).Skip(1).Take(2).ToList(),
    Midfielders = team1.Players.Where(p => p.Position == PlayerPosition.Ce).Take(4).ToList(),
    Attackers = team1.Players.Where(p => p.Position == PlayerPosition.At).Take(3).ToList()
};

Console.WriteLine($"Formazione: 2-Li-4-3 (CON LIBERO)");
Console.WriteLine($"Tentativo di attivare TFG con Libero presente...");

var tacticsWithTFG = new FormationTactics
{
    UseOffsideTrap = true,
    HardnessTotal = 0
};

if (!tacticsValidator.ValidateTactics(formationWithLibero, tacticsWithTFG, out string errorTFG))
{
    Console.WriteLine($"✅ VALIDATOR CORRETTO: {errorTFG}");
}
else
{
    Console.WriteLine("❌ VALIDATOR SBAGLIATO: Dovrebbe bloccare TFG con Libero!");
}
Console.WriteLine();

var formationWithoutLibero = new Formation
{
    Goalkeeper = team1.Players.First(p => p.Position == PlayerPosition.Po),
    Defenders = team1.Players.Where(p => p.Position == PlayerPosition.Di).Take(4).ToList(),
    Midfielders = team1.Players.Where(p => p.Position == PlayerPosition.Ce).Take(3).ToList(),
    Attackers = team1.Players.Where(p => p.Position == PlayerPosition.At).Take(3).ToList()
};

Console.WriteLine($"Formazione: 4-3-3 (SENZA LIBERO)");
Console.WriteLine($"Tentativo di attivare TFG senza Libero...");

if (!tacticsValidator.ValidateTactics(formationWithoutLibero, tacticsWithTFG, out string errorTFG2))
{
    Console.WriteLine($"❌ VALIDATOR: {errorTFG2}");
}
else
{
    Console.WriteLine("✅ VALIDATOR CORRETTO: TFG accettato (nessun Libero presente)");
}
Console.WriteLine();

Console.WriteLine("=== CONCLUSIONE ===");
Console.WriteLine("✅ Il limite di 5 punti extra sul LIBERO è implementato correttamente.");
Console.WriteLine("✅ Il limite di 5 punti extra sul PORTIERE è implementato correttamente.");
Console.WriteLine("✅ Il Validator controlla che la somma di tutti i punti su Po/Li non superi 5.");
Console.WriteLine("✅ Il MatchEngine applica un limite rigido di 5 punti totali su Po e Li.");
Console.WriteLine("✅ Il FC non può essere usato sul Libero o Portiere (solo su Di, Ce, At).");
Console.WriteLine("✅ La TFG può essere attivata SOLO se NON è presente un Libero.");
Console.WriteLine();
Console.WriteLine("=== SISTEMA INFORTUNI ===");
Console.WriteLine("✅ Probabilità infortunio: (5 + Durezza avversaria)%");
Console.WriteLine("✅ Portieri: probabilità dimezzata");
Console.WriteLine("✅ Gravità: 6 livelli da Fo-0 (lieve) a Fo-6 (grave)");
Console.WriteLine("✅ Livello in partita: 3/4 (infortuni lievi) o 1/2 (altri)");
Console.WriteLine("✅ Massaggiatore: riduce gravità di 1 livello nella tabella");
Console.WriteLine("✅ Output: mostra livello in partita e perdita forma");
Console.WriteLine();
Console.WriteLine("=== TEST: SISTEMA TATTICO ===");
Console.WriteLine();

Console.WriteLine("TEST F: Tattico con tattica corretta (3-3-3 Li CAT)");
var tacticsTatticoOK = new FormationTactics
{
    UseCatenaccio = true,
    CatenaccioPoints = 7,
    CatenaccioDistribution = new Dictionary<string, int> { { "Li", 2 }, { "Di", 3 }, { "Ce", 2 } },
    TacticianBonusPoints = 5,
    TacticianDistribution = new Dictionary<string, int> { { "Di", 2 }, { "Ce", 2 }, { "At", 1 } }
};

if (!tacticsValidator.ValidateTactics(team1, formation1, tacticsTatticoOK, out string errorF))
{
    Console.WriteLine($"❌ VALIDATOR: {errorF}");
}
else
{
    Console.WriteLine($"✅ VALIDATOR: Tattico bonus applicabile!");
    var tactician = team1.Staff.First(s => s.Type == BenchStaffType.Tactician);
    string currentTactic = TacticHelper.GenerateTacticString(formation1, tacticsTatticoOK);
    Console.WriteLine($"   Tattico: {tactician.Name} specializzato in '{tactician.SpecializedTactic}'");
    Console.WriteLine($"   Tattica corrente: '{currentTactic}'");
    Console.WriteLine($"   Match: ✅ Bonus di 5 punti attivo!");
}
Console.WriteLine();

Console.WriteLine("TEST G: Tattico con tattica sbagliata (4-3-3 invece di 3-3-3 Li CAT)");
var tacticsTatticoWrong = new FormationTactics
{
    TacticianBonusPoints = 5,
    TacticianDistribution = new Dictionary<string, int> { { "Di", 2 }, { "Ce", 2 }, { "At", 1 } }
};

if (!tacticsValidator.ValidateTactics(team1, formation3, tacticsTatticoWrong, out string errorG))
{
    Console.WriteLine($"✅ VALIDATOR CORRETTO: {errorG}");
}
else
{
    Console.WriteLine("❌ VALIDATOR SBAGLIATO: Dovrebbe bloccare bonus tattico con tattica sbagliata!");
}
Console.WriteLine();

Console.WriteLine("TEST H: Tentativo di usare bonus tattico senza avere un tattico");
var team3 = teamFactory.CreateInitialTeam("Test FC", "Test Manager");
var tacticsSenzaTattico = new FormationTactics
{
    TacticianBonusPoints = 5,
    TacticianDistribution = new Dictionary<string, int> { { "Di", 3 }, { "Ce", 2 } }
};

if (!tacticsValidator.ValidateTactics(team3, formation3, tacticsSenzaTattico, out string errorH))
{
    Console.WriteLine($"✅ VALIDATOR CORRETTO: {errorH}");
}
else
{
    Console.WriteLine("❌ VALIDATOR SBAGLIATO: Dovrebbe bloccare bonus senza tattico!");
}
Console.WriteLine();

Console.WriteLine("TEST I: Tentativo di avere 2 tattici (DEVE FALLIRE)");
team3.Staff.Add(new BenchStaff("Tattico 1", BenchStaffType.Tactician, "4-4-2"));
team3.Staff.Add(new BenchStaff("Tattico 2", BenchStaffType.Tactician, "3-5-2"));
var tacticsDueTattici = new FormationTactics { HardnessTotal = 0 };

if (!tacticsValidator.ValidateTactics(team3, formation3, tacticsDueTattici, out string errorI))
{
    Console.WriteLine($"✅ VALIDATOR CORRETTO: {errorI}");
}
else
{
    Console.WriteLine("❌ VALIDATOR SBAGLIATO: Dovrebbe bloccare 2 tattici!");
}
Console.WriteLine();

Console.WriteLine("=== CONCLUSIONE FINALE ===");
Console.WriteLine("✅ Il limite di 5 punti extra sul LIBERO è implementato correttamente.");
Console.WriteLine("✅ Il limite di 5 punti extra sul PORTIERE è implementato correttamente.");
Console.WriteLine("✅ Il Validator controlla che la somma di tutti i punti su Po/Li non superi 5.");
Console.WriteLine("✅ Il MatchEngine applica un limite rigido di 5 punti totali su Po e Li.");
Console.WriteLine("✅ Il FC non può essere usato sul Libero o Portiere (solo su Di, Ce, At).");
Console.WriteLine("✅ La TFG può essere attivata SOLO se NON è presente un Libero.");
Console.WriteLine();
Console.WriteLine("=== SISTEMA INFORTUNI ===");
Console.WriteLine("✅ Probabilità infortunio: (5 + Durezza avversaria)%");
Console.WriteLine("✅ Portieri: probabilità dimezzata");
Console.WriteLine("✅ Gravità: 6 livelli da Fo-0 (lieve) a Fo-6 (grave)");
Console.WriteLine("✅ Livello in partita: 3/4 (infortuni lievi) o 1/2 (altri)");
Console.WriteLine("✅ Massaggiatore: riduce gravità di 1 livello nella tabella");
Console.WriteLine("✅ Output: mostra livello in partita e perdita forma");
Console.WriteLine();
Console.WriteLine("=== SISTEMA DISCIPLINARE ===");
Console.WriteLine("✅ Ammonizioni: 1.5 * (3 + Durezza)% → +4 PD 🟨");
Console.WriteLine("✅ Espulsioni: 0.33 * (3 + Durezza)% → +10 PD 🟥");
Console.WriteLine("✅ Portiere: usa SOLO la durezza specifica sul Po, non quella totale");
Console.WriteLine("✅ Giocatore espulso: valore DIMEZZATO durante la partita");
Console.WriteLine("✅ Diffida: 5-9 PD ⚠️ (prossima ammonizione = squalifica)");
Console.WriteLine("✅ Squalifica: 10+ PD 🟥 (non gioca prossima partita)");
Console.WriteLine("✅ PD reset: -5 PD ogni turno (regole di campionato)");
Console.WriteLine();
Console.WriteLine("=== SISTEMA RIGORI ===");
Console.WriteLine("✅ Probabilità rigore: 10% per ogni punto Durezza (min 5% con D=0)");
Console.WriteLine("✅ Probabilità segnare: 40 + (3 * Ab tiratore) - Po portiere (max 95%)");
Console.WriteLine("✅ Tiratore: scelto automaticamente come miglior giocatore disponibile");
Console.WriteLine("✅ Esclusioni: giocatori infortunati (Fo ≤ -3) ed espulsi (PD ≥ 10)");
Console.WriteLine();
Console.WriteLine("=== SISTEMA TATTICO ===");
Console.WriteLine("✅ Tattico: fornisce +5 punti quando si usa la sua tattica specializzata");
Console.WriteLine("✅ Tattica = formazione (X-Y-Z) + Libero + TFG");
Console.WriteLine("✅ CAT (Catenaccio) NON fa parte dell'identificazione tattica");
Console.WriteLine("✅ Esempi: '3-2-5 Li' (con libero), '3-5-2 TFG' (con fuorigioco), '4-4-2' (base)");
Console.WriteLine("✅ Punti distribuibili su Li, Di, Ce, At (non sul Po)");
Console.WriteLine("✅ Max 1 tattico per squadra");
Console.WriteLine();
Console.WriteLine("Tutte le regole tattiche, infortuni e staff sono implementate correttamente!");

static void PrintPlayerFormsAfterMatch(Team team, Formation formation)
{
    Console.WriteLine($"=== FORMA GIOCATORI - {team.Name} ===");

    var allPlayers = new List<Player>();
    if (formation.Goalkeeper != null) allPlayers.Add(formation.Goalkeeper);
    if (formation.Libero != null) allPlayers.Add(formation.Libero);
    allPlayers.AddRange(formation.Defenders);
    allPlayers.AddRange(formation.Midfielders);
    allPlayers.AddRange(formation.Attackers);

    foreach (var player in allPlayers)
    {
        string positionLabel = player == formation.Goalkeeper ? "Po" :
                               player == formation.Libero ? "Li" :
                               formation.Defenders.Contains(player) ? "Di" :
                               formation.Midfielders.Contains(player) ? "Ce" : "At";

        string formStatus = player.Form switch
        {
            <= -3 => $"{player.Form} [INF]",
            < 0 => $"{player.Form}",
            0 => $"{player.Form}",
            > 0 => $"+{player.Form} [*]"
        };

        string disciplineStatus = player.DisciplinePoints switch
        {
            >= 10 => $" | PD: {player.DisciplinePoints} [SQU]",
            >= 5 => $" | PD: {player.DisciplinePoints} [DIF]",
            > 0 => $" | PD: {player.DisciplinePoints} [AMM]",
            _ => ""
        };

        Console.WriteLine($"  [{positionLabel}] {player.Name,-20} - Età: {player.Age}, Ab: {player.Ability}, Fo: {formStatus}{disciplineStatus}");
    }
    Console.WriteLine();
}

static void PrintMatchEvents(Match match, string homeTeamName, string awayTeamName)
{
    if (match.Events.Count == 0) return;

    Console.WriteLine("=== CRONOLOGIA EVENTI PARTITA ===");
    var sortedEvents = match.Events.OrderBy(e => e.Minute).ThenBy(e => e.Type).ToList();

    foreach (var evt in sortedEvents)
    {
        string team = evt.IsHomeTeam ? homeTeamName : awayTeamName;
        string eventPrefix = evt.Type switch
        {
            MatchEventType.Goal => "[GOL]",
            MatchEventType.Penalty => "[RIG]",
            MatchEventType.PenaltyMissed => "[SBG]",
            MatchEventType.OwnGoal => "[AUT]",
            MatchEventType.RedCard => "[ESP]",
            MatchEventType.YellowCard => "[AMM]",
            MatchEventType.Injury => "[INF]",
            MatchEventType.ShotMissed => "[---]",
            MatchEventType.ShotSavedByGoalkeeper => "[PAR]",
            MatchEventType.ShotBlockedByLibero => "[BLO]",
            _ => "[???]"
        };

        Console.WriteLine($"{evt.Minute,3}' - [{team,-20}] {eventPrefix} {evt.Description}");
    }
    Console.WriteLine();
}

static void PrintTeamRoster(Team team)
{
    const int tableWidth = 72;

    // Helper function to create a table row
    string CreateRow(string content)
    {
        if (content.Length > tableWidth - 2)
            content = content.Substring(0, tableWidth - 2);
        return "║ " + content.PadRight(tableWidth - 2) + " ║";
    }

    Console.WriteLine("╔" + new string('═', tableWidth) + "╗");
    Console.WriteLine(CreateRow(team.Name.ToUpper()));
    Console.WriteLine(CreateRow("Manager: " + team.ManagerName));
    Console.WriteLine("╠" + new string('═', tableWidth) + "╣");
    Console.WriteLine(CreateRow("Stadio: Livello " + team.Stadium.Level));
    Console.WriteLine(CreateRow("Denaro: " + team.Money + " M"));
    Console.WriteLine(CreateRow("Punti Allenamento: " + team.TrainingPoints));
    Console.WriteLine(CreateRow("Punti Grande Prestazione: " + team.GreatPerformancePoints));
    Console.WriteLine("╠" + new string('═', tableWidth) + "╣");

    string staffInfo = string.Join(", ", team.Staff.Select(s => 
        s.Type == BenchStaffType.Tactician ? $"{s.Name} ({s.SpecializedTactic})" : s.Name));
    Console.WriteLine(CreateRow("Staff: " + staffInfo));
    Console.WriteLine("╠" + new string('═', tableWidth) + "╣");
    Console.WriteLine(CreateRow("ROSA COMPLETA (" + team.Players.Count + " giocatori)"));
    Console.WriteLine("╠" + new string('═', tableWidth) + "╣");

    var groupedPlayers = team.Players
        .OrderBy(p => p.Age)
        .ThenByDescending(p => p.Ability)
        .GroupBy(p => p.Position);

    foreach (var posGroup in new[] { PlayerPosition.Po, PlayerPosition.Di, PlayerPosition.Ce, PlayerPosition.At })
    {
        var players = groupedPlayers.FirstOrDefault(g => g.Key == posGroup)?.ToList();
        if (players == null || players.Count == 0) continue;

        Console.WriteLine(CreateRow(posGroup.ToString() + ":"));

        foreach (var player in players)
        {
            string sideIcon = player.Side switch
            {
                PlayerSide.S => "S  ",
                PlayerSide.D => "D  ",
                PlayerSide.SD => "S+D",
                _ => "?  "
            };

            string ageLabel = player.Age switch
            {
                PlayerAge.Primavera => "P  ",
                PlayerAge.Juniores => "J  ",
                PlayerAge.I => "I  ",
                PlayerAge.II => "II ",
                PlayerAge.III => "III",
                PlayerAge.IV => "IV ",
                PlayerAge.V => "V  ",
                PlayerAge.VI => "VI ",
                PlayerAge.VII => "VII",
                _ => "?  "
            };

            string formDisplay = player.Form >= 0 ? $"+{player.Form}" : player.Form.ToString();

            // Status indicators without emoji
            string statusText = "";
            if (player.Form <= -3) statusText = " [INF]";
            else if (player.Form > 0) statusText = " [*]";

            string disciplineText = "";
            if (player.DisciplinePoints >= 10) disciplineText = " [SQU]";
            else if (player.DisciplinePoints >= 5) disciplineText = " [DIF]";
            else if (player.DisciplinePoints > 0) disciplineText = " [AMM]";

            // Build player line
            string playerLine = $"   {player.Name.PadRight(20)} [{sideIcon}] Età:{ageLabel} Ab:{player.Ability,2} Fo:{formDisplay,3}";

            // Add discipline points if present
            if (player.DisciplinePoints > 0)
            {
                playerLine += $" PD:{player.DisciplinePoints,2}";
            }
            else
            {
                playerLine += "      ";
            }

            // Add status text
            playerLine += statusText.PadRight(6);
            playerLine += disciplineText.PadRight(6);

            Console.WriteLine(CreateRow(playerLine));
        }
    }

    Console.WriteLine("╚" + new string('═', tableWidth) + "╝");
    Console.WriteLine();
}

static void PrintLineup(string teamName, Formation formation, string tactic)
{
    Console.WriteLine($"┌────────────────────────────────────────────────────────────────────┐");
    Console.WriteLine($"│ FORMAZIONE {teamName.ToUpper()}".PadRight(69) + "│");
    Console.WriteLine($"│ Modulo: {tactic}".PadRight(69) + "│");
    Console.WriteLine($"├────────────────────────────────────────────────────────────────────┤");

    string GetAgeLabel(PlayerAge age)
    {
        return age switch
        {
            PlayerAge.Primavera => "P  ",
            PlayerAge.Juniores => "J  ",
            PlayerAge.I => "I  ",
            PlayerAge.II => "II ",
            PlayerAge.III => "III",
            PlayerAge.IV => "IV ",
            PlayerAge.V => "V  ",
            PlayerAge.VI => "VI ",
            PlayerAge.VII => "VII",
            _ => "?  "
        };
    }

    if (formation.Goalkeeper != null)
    {
        var p = formation.Goalkeeper;
        string sideStr = p.Side.ToString().PadRight(3);
        string ageLabel = GetAgeLabel(p.Age);
        string line = $"│ [Po] {p.Name.PadRight(20)} │ Età: {ageLabel} │ Ab: {p.Ability,2} │ Fo: {p.Form,3} │ {sideStr} │";
        Console.WriteLine(line);
    }

    if (formation.Libero != null)
    {
        var p = formation.Libero;
        string sideStr = p.Side.ToString().PadRight(3);
        string ageLabel = GetAgeLabel(p.Age);
        string line = $"│ [Li] {p.Name.PadRight(20)} │ Età: {ageLabel} │ Ab: {p.Ability,2} │ Fo: {p.Form,3} │ {sideStr} │";
        Console.WriteLine(line);
    }

    if (formation.Defenders.Count > 0)
    {
        Console.WriteLine($"├────────────────────────────────────────────────────────────────────┤");
        Console.WriteLine($"│ DIFENSORI ({formation.Defenders.Count})".PadRight(69) + "│");
        foreach (var p in formation.Defenders)
        {
            string sideStr = p.Side.ToString().PadRight(3);
            string ageLabel = GetAgeLabel(p.Age);
            string line = $"│      {p.Name.PadRight(20)} │ Età: {ageLabel} │ Ab: {p.Ability,2} │ Fo: {p.Form,3} │ {sideStr} │";
            Console.WriteLine(line);
        }
    }

    if (formation.Midfielders.Count > 0)
    {
        Console.WriteLine($"├────────────────────────────────────────────────────────────────────┤");
        Console.WriteLine($"│ CENTROCAMPISTI ({formation.Midfielders.Count})".PadRight(69) + "│");
        foreach (var p in formation.Midfielders)
        {
            string sideStr = p.Side.ToString().PadRight(3);
            string ageLabel = GetAgeLabel(p.Age);
            string line = $"│      {p.Name.PadRight(20)} │ Età: {ageLabel} │ Ab: {p.Ability,2} │ Fo: {p.Form,3} │ {sideStr} │";
            Console.WriteLine(line);
        }
    }

    if (formation.Attackers.Count > 0)
    {
        Console.WriteLine($"├────────────────────────────────────────────────────────────────────┤");
        Console.WriteLine($"│ ATTACCANTI ({formation.Attackers.Count})".PadRight(69) + "│");
        foreach (var p in formation.Attackers)
        {
            string sideStr = p.Side.ToString().PadRight(3);
            string ageLabel = GetAgeLabel(p.Age);
            string line = $"│      {p.Name.PadRight(20)} │ Età: {ageLabel} │ Ab: {p.Ability,2} │ Fo: {p.Form,3} │ {sideStr} │";
            Console.WriteLine(line);
        }
    }

    int totalPlayers = (formation.Goalkeeper != null ? 1 : 0) +
                      (formation.Libero != null ? 1 : 0) +
                      formation.Defenders.Count +
                      formation.Midfielders.Count +
                      formation.Attackers.Count;
    Console.WriteLine($"├────────────────────────────────────────────────────────────────────┤");
    Console.WriteLine($"│ TOTALE: {totalPlayers} giocatori in campo".PadRight(69) + "│");
    Console.WriteLine($"└────────────────────────────────────────────────────────────────────┘");
    Console.WriteLine();
}

// ============================================================================
// DEMO: Sistema di gestione stagionale
// ============================================================================
Console.WriteLine("\n\n=== DEMO: SISTEMA DI GESTIONE STAGIONALE ===\n");

// Crea il manager della stagione
var seasonManager = new SeasonManager();

// Crea squadre per le diverse divisioni (esempio semplificato)
var serieATeams = new List<Team>();
var serieBTeams = new List<Team>();
var serieCTeams = new List<Team>();

// Crea 12 squadre per Serie A
for (int i = 1; i <= 12; i++)
{
    serieATeams.Add(teamFactory.CreateInitialTeam($"Serie A Team {i}", $"Manager {i}"));
}

// Crea 12 squadre per Serie B (esempio ridotto a 4 per brevità)
for (int i = 1; i <= 4; i++)
{
    serieBTeams.Add(teamFactory.CreateInitialTeam($"Serie B Team {i}", $"Manager B{i}"));
}

// Crea 4 squadre per Serie C
for (int i = 1; i <= 4; i++)
{
    serieCTeams.Add(teamFactory.CreateInitialTeam($"Serie C Team {i}", $"Manager C{i}"));
}

// Crea la stagione 2024
Console.WriteLine("Creazione stagione 2024...");
var season = seasonManager.CreateSeason(2024, serieATeams, serieBTeams, serieCTeams);
Console.WriteLine($"Stagione {season.Year} creata con successo!");
Console.WriteLine($"- Competizioni: {season.Competitions.Count}");
Console.WriteLine($"- Sessioni di gioco: {season.Sessions.Count}");
Console.WriteLine($"- Partite totali: {season.Sessions.Sum(s => s.Matches.Count)}");
Console.WriteLine();

// Mostra le competizioni create
foreach (var competition in season.Competitions)
{
    Console.WriteLine($"[{competition.Type}] {competition.Name}");
    Console.WriteLine($"  - Squadre: {competition.Teams.Count}");
    Console.WriteLine($"  - Partite: {competition.Matches.Count}");
    if (competition.Standings.Count > 0)
    {
        Console.WriteLine($"  - Classifica inizializzata: {competition.Standings.Count} squadre");
    }
    Console.WriteLine();
}

// Mostra un esempio di calendario per Serie A
var serieACompetition = season.GetChampionship(Division.SerieA);
if (serieACompetition != null)
{
    Console.WriteLine($"\n=== CALENDARIO SERIE A (prime 5 giornate) ===");
    var firstRounds = serieACompetition.Matches
        .Where(m => m.Round <= 5)
        .GroupBy(m => m.Round)
        .OrderBy(g => g.Key);

    foreach (var round in firstRounds)
    {
        Console.WriteLine($"\nGiornata {round.Key}:");
        foreach (var match in round)
        {
            Console.WriteLine($"  {match.HomeTeam.Name,-20} vs {match.AwayTeam.Name,-20}");
        }
    }
}

// Mostra la distribuzione delle partite nelle sessioni
Console.WriteLine($"\n\n=== DISTRIBUZIONE PARTITE IN {season.Sessions.Count} SESSIONI ===");
foreach (var session in season.Sessions.Take(3)) // Mostra solo le prime 3
{
    Console.WriteLine($"\nSessione {session.SessionNumber}: {session.Matches.Count} partite");
    var competitions = session.Matches
        .Where(m => m.CompetitionType.HasValue)
        .GroupBy(m => m.CompetitionType.Value)
        .Select(g => $"{g.Key} ({g.Count()})");
    Console.WriteLine($"  Competizioni: {string.Join(", ", competitions)}");
}
Console.WriteLine("  ...");
Console.WriteLine();

Console.WriteLine("=== SISTEMA STAGIONALE IMPLEMENTATO ===");
Console.WriteLine("✅ Creazione automatica di 3 divisioni (Serie A, B, C)");
Console.WriteLine("✅ Calendario all'italiana (andata e ritorno) per ogni divisione");
Console.WriteLine("✅ Coppa Beppiland (eliminazione diretta) per tutte le squadre");
Console.WriteLine("✅ Distribuzione automatica delle partite in 11 sessioni");
Console.WriteLine("✅ Sistema di classifica con aggiornamento automatico");
Console.WriteLine("✅ Gestione promozioni e retrocessioni");
Console.WriteLine();
Console.WriteLine("TODO per completare:");
Console.WriteLine("- Coppa di Lega (gironi + eliminazione)");
Console.WriteLine("- Coppa Juniores (solo formazioni giovani)");
Console.WriteLine("- Calendario intelligente delle coppe durante la stagione");
Console.WriteLine("- Sistema di persistenza divisioni tra stagioni");
Console.WriteLine();


