namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;

public class SeasonManager
{
    private readonly CalendarGenerator _calendarGenerator = new();

    /// <summary>
    /// Crea una nuova stagione con tutte le competizioni
    /// </summary>
    public Season CreateSeason(int year, 
        List<Team> serieATeams, 
        List<Team> serieBTeams, 
        List<Team> serieCTeams)
    {
        var season = new Season(year);

        // Reset PGP a 30 per tutte le squadre (non trasferibili tra stagioni)
        var allTeams = serieATeams.Concat(serieBTeams).Concat(serieCTeams).ToList();
        foreach (var team in allTeams)
        {
            team.GreatPerformancePoints = 30;
        }

        // Crea i campionati solo per le divisioni con almeno 2 squadre (numero pari)
        if (serieATeams.Count >= 2 && serieATeams.Count % 2 == 0)
        {
            season.Competitions.Add(CreateChampionship("Serie A", Division.SerieA, serieATeams));
        }

        if (serieBTeams.Count >= 2 && serieBTeams.Count % 2 == 0)
        {
            season.Competitions.Add(CreateChampionship("Serie B", Division.SerieB, serieBTeams));
        }

        if (serieCTeams.Count >= 2 && serieCTeams.Count % 2 == 0)
        {
            season.Competitions.Add(CreateChampionship("Serie C", Division.SerieC, serieCTeams));
        }

        // Crea le coppe solo se ci sono squadre sufficienti
        // Coppa Beppiland - eliminazione diretta (serve numero pari di squadre)
        if (allTeams.Count >= 2 && allTeams.Count % 2 == 0)
        {
            season.Competitions.Add(CreateBeppilandCup(allTeams));
        }

        // TODO: Coppa di Lega e Coppa Juniores (implementazione futura)

        // Distribuisce le partite in sessioni
        DistributeMatchesIntoSessions(season);

        return season;
    }

    /// <summary>
    /// Crea un campionato
    /// </summary>
    private Competition CreateChampionship(string name, Division division, List<Team> teams)
    {
        var competition = new Competition(name, CompetitionType.Championship, division)
        {
            Teams = new List<Team>(teams)
        };

        // Genera calendario andata e ritorno
        competition.Matches = _calendarGenerator.GenerateChampionshipCalendar(teams, CompetitionType.Championship);

        // Inizializza classifica
        foreach (var team in teams)
        {
            competition.Standings.Add(new Standing(team));
        }

        return competition;
    }

    /// <summary>
    /// Crea la Coppa Beppiland (eliminazione diretta)
    /// </summary>
    private Competition CreateBeppilandCup(List<Team> teams)
    {
        var competition = new Competition("Coppa Beppiland", CompetitionType.BeppilandCup)
        {
            Teams = new List<Team>(teams)
        };

        // Primo turno (tutti contro tutti a eliminazione)
        // Assumiamo squadre multiple di 2 per semplicità
        if (teams.Count % 2 == 0)
        {
            competition.Matches = _calendarGenerator.GenerateKnockoutRound(
                teams, CompetitionType.BeppilandCup, 1, "Primo Turno");
        }

        return competition;
    }

    /// <summary>
    /// Distribuisce tutte le partite della stagione in 11 sessioni
    /// </summary>
    private void DistributeMatchesIntoSessions(Season season)
    {
        // Raccoglie tutte le partite di tutte le competizioni
        var allMatches = new List<Match>();

        foreach (var competition in season.Competitions)
        {
            allMatches.AddRange(competition.Matches);
        }

        // Distribuisce in 11 sessioni
        season.Sessions = _calendarGenerator.DistributeMatchesIntoSessions(allMatches, 11);
    }

    /// <summary>
    /// Aggiorna le classifiche dopo una partita
    /// </summary>
    public void UpdateStandings(Competition competition, Match match)
    {
        if (!match.IsPlayed) return;

        var homeStanding = competition.Standings.FirstOrDefault(s => s.Team == match.HomeTeam);
        var awayStanding = competition.Standings.FirstOrDefault(s => s.Team == match.AwayTeam);

        homeStanding?.AddMatch(match.HomeGoals, match.AwayGoals);
        awayStanding?.AddMatch(match.AwayGoals, match.HomeGoals);

        // Ordina la classifica
        competition.Standings = competition.Standings
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .ToList();
    }

    /// <summary>
    /// Gestisce promozioni e retrocessioni a fine stagione
    /// </summary>
    public void ProcessPromotionsAndRelegations(Season season)
    {
        var serieA = season.GetChampionship(Division.SerieA);
        var serieB = season.GetChampionship(Division.SerieB);
        var serieC = season.GetChampionship(Division.SerieC);

        if (serieA == null || serieB == null || serieC == null)
            return;

        // Ultime 3 di Serie A retrocedono in B
        var relegatedFromA = serieA.Standings.TakeLast(3).Select(s => s.Team).ToList();

        // Prime 3 di Serie B promosse in A
        var promotedFromB = serieB.Standings.Take(3).Select(s => s.Team).ToList();

        // Ultime 3 di Serie B retrocedono in C
        var relegatedFromB = serieB.Standings.TakeLast(3).Select(s => s.Team).ToList();

        // Prime 3 di Serie C promosse in B
        var promotedFromC = serieC.Standings.Take(3).Select(s => s.Team).ToList();

        // Aggiorna le divisioni (salva per la prossima stagione)
        // TODO: Implementare sistema di persistenza divisioni
    }

    public void AgePlayer(Player player)
    {
        switch (player.Age)
        {
            case PlayerAge.Primavera:
                player.Age = PlayerAge.Juniores;
                break;
            case PlayerAge.Juniores:
                if (player.Ability >= 5)
                {
                    player.Age = PlayerAge.I;
                }
                break;
            case PlayerAge.I:
                player.Age = PlayerAge.II;
                break;
            case PlayerAge.II:
                player.Age = PlayerAge.III;
                break;
            case PlayerAge.III:
                player.Age = PlayerAge.IV;
                break;
            case PlayerAge.IV:
                player.Age = PlayerAge.V;
                break;
            case PlayerAge.V:
                player.Age = PlayerAge.VI;
                break;
            case PlayerAge.VI:
                player.Age = PlayerAge.VII;
                break;
        }
    }

    public Player CreatePrimavera(string name, PlayerPosition position, PlayerSide side)
    {
        return new Player(name, position, 2, PlayerAge.Primavera)
        {
            Form = 2,
            Side = side
        };
    }
}
