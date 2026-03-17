namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;

public class CalendarGenerator
{
    private readonly Random _random = new();

    /// <summary>
    /// Genera il calendario per un campionato all'italiana (andata e ritorno)
    /// </summary>
    public List<Match> GenerateChampionshipCalendar(List<Team> teams, CompetitionType competitionType)
    {
        var matches = new List<Match>();
        int teamCount = teams.Count;

        if (teamCount < 2 || teamCount % 2 != 0)
            throw new ArgumentException("Il numero di squadre deve essere pari e almeno 2");

        // Algoritmo Round-Robin per generare tutti gli incontri
        var rounds = GenerateRoundRobin(teams);

        // Andata
        int roundNumber = 1;
        foreach (var round in rounds)
        {
            foreach (var (home, away) in round)
            {
                var match = new Match(home, away)
                {
                    CompetitionType = competitionType,
                    Round = roundNumber,
                    Description = $"Giornata {roundNumber}"
                };
                matches.Add(match);
            }
            roundNumber++;
        }

        // Ritorno (invertendo casa/trasferta)
        foreach (var round in rounds)
        {
            foreach (var (home, away) in round)
            {
                var match = new Match(away, home)
                {
                    CompetitionType = competitionType,
                    Round = roundNumber,
                    Description = $"Giornata {roundNumber}"
                };
                matches.Add(match);
            }
            roundNumber++;
        }

        return matches;
    }

    /// <summary>
    /// Genera turni per torneo eliminazione diretta
    /// </summary>
    public List<Match> GenerateKnockoutRound(List<Team> teams, CompetitionType competitionType, int roundNumber, string roundName)
    {
        var matches = new List<Match>();
        
        if (teams.Count % 2 != 0)
            throw new ArgumentException("Il numero di squadre deve essere pari");

        // Mescola le squadre per sorteggio casuale
        var shuffledTeams = teams.OrderBy(x => _random.Next()).ToList();

        for (int i = 0; i < shuffledTeams.Count; i += 2)
        {
            var match = new Match(shuffledTeams[i], shuffledTeams[i + 1])
            {
                CompetitionType = competitionType,
                Round = roundNumber,
                Description = roundName
            };
            matches.Add(match);
        }

        return matches;
    }

    /// <summary>
    /// Algoritmo Round-Robin per generare il calendario
    /// </summary>
    private List<List<(Team, Team)>> GenerateRoundRobin(List<Team> teams)
    {
        int n = teams.Count;
        var rounds = new List<List<(Team, Team)>>();

        // Crea una copia della lista per non modificare l'originale
        var teamList = new List<Team>(teams);

        // Algoritmo round-robin: fissiamo una squadra e ruotiamo le altre
        for (int round = 0; round < n - 1; round++)
        {
            var roundMatches = new List<(Team, Team)>();

            for (int i = 0; i < n / 2; i++)
            {
                int home = i;
                int away = n - 1 - i;

                roundMatches.Add((teamList[home], teamList[away]));
            }

            rounds.Add(roundMatches);

            // Rotazione: la prima squadra resta fissa, le altre ruotano
            var temp = teamList[1];
            for (int i = 1; i < n - 1; i++)
            {
                teamList[i] = teamList[i + 1];
            }
            teamList[n - 1] = temp;
        }

        return rounds;
    }

    /// <summary>
    /// Distribuisce le partite in sessioni (generalmente 2-4 partite per sessione)
    /// </summary>
    public List<GameSession> DistributeMatchesIntoSessions(List<Match> allMatches, int targetSessions = 11)
    {
        var sessions = new List<GameSession>();
        int matchesPerSession = (int)Math.Ceiling((double)allMatches.Count / targetSessions);

        for (int i = 0; i < targetSessions; i++)
        {
            var session = new GameSession(i + 1);
            
            int startIndex = i * matchesPerSession;
            int count = Math.Min(matchesPerSession, allMatches.Count - startIndex);
            
            if (count > 0)
            {
                session.Matches = allMatches.GetRange(startIndex, count);
                sessions.Add(session);
            }
        }

        return sessions;
    }
}
