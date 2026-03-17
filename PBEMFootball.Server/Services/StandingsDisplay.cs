namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;

public class StandingsDisplay
{
    private const int TABLE_WIDTH = 72;

    private static string CreateTableRow(string content)
    {
        int contentLength = content.Length;
        int padding = TABLE_WIDTH - contentLength;

        if (padding < 0)
        {
            content = content.Substring(0, TABLE_WIDTH);
            padding = 0;
        }

        return $"║{content}{new string(' ', padding)}║";
    }
    public static void PrintStandings(Competition competition)
    {
        Console.WriteLine($"\n╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow($" CLASSIFICA: {competition.Name.ToUpper()} "));
        Console.WriteLine($"╠════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine($"║ Pos  Squadra                G   V  P  S  GF  GS  DR  Punti            ║");
        Console.WriteLine($"╠════════════════════════════════════════════════════════════════════════╣");

        int position = 1;
        foreach (var standing in competition.Standings)
        {
            string positionMarker = "";
            
            if (competition.Division == Division.SerieA)
            {
                if (position >= competition.Standings.Count - 2)
                    positionMarker = "[RET]"; // Retrocessione
            }
            else if (competition.Division == Division.SerieB)
            {
                if (position <= 3)
                    positionMarker = "[PRO]"; // Promozione
                else if (position >= competition.Standings.Count - 2)
                    positionMarker = "[RET]"; // Retrocessione
            }
            else if (competition.Division == Division.SerieC)
            {
                if (position <= 3)
                    positionMarker = "[PRO]"; // Promozione
            }

            string content = $" {position,2}.  {standing.Team.Name,-20} " +
                            $"{standing.Played,2}  {standing.Won,2} {standing.Drawn,2} {standing.Lost,2} " +
                            $"{standing.GoalsFor,3} {standing.GoalsAgainst,3} {standing.GoalDifference,4} " +
                            $"{standing.Points,3} {positionMarker,-6} ";

            Console.WriteLine(CreateTableRow(content));
            position++;
        }

        Console.WriteLine($"╚════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine($"Legenda: G=Giocate, V=Vinte, P=Pareggiate, S=Sconfitte");
        Console.WriteLine($"         GF=Goal Fatti, GS=Goal Subiti, DR=Differenza Reti");
        Console.WriteLine($"         [PRO]=Zona Promozione, [RET]=Zona Retrocessione");
        Console.WriteLine();
    }

    public static void PrintSessionSummary(GameSession session)
    {
        Console.WriteLine($"\n╔════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine(CreateTableRow($" SESSIONE {session.SessionNumber} "));
        Console.WriteLine($"╠════════════════════════════════════════════════════════════════════════╣");

        foreach (var match in session.Matches)
        {
            if (match.IsPlayed)
            {
                string result = $"{match.HomeTeam.Name,-20} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam.Name,-20}";
                string competition = match.CompetitionType?.ToString() ?? "N/A";
                string content = $" {result}  [{competition,-15}] ";
                Console.WriteLine(CreateTableRow(content));
            }
            else
            {
                string fixture = $"{match.HomeTeam.Name,-20} vs {match.AwayTeam.Name,-20}";
                string competition = match.CompetitionType?.ToString() ?? "N/A";
                string content = $" {fixture}  [{competition,-15}] ";
                Console.WriteLine(CreateTableRow(content));
            }
        }

        Console.WriteLine($"╚════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
    }
}
