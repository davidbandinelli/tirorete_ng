namespace PBEMFootball.Common.Models;

public class GameSession
{
    public int SessionNumber { get; set; }
    public List<Match> Matches { get; set; } = new();
    public bool IsCompleted { get; set; }

    public GameSession(int sessionNumber)
    {
        SessionNumber = sessionNumber;
    }
}
