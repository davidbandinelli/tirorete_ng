namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;

public class GameEngine
{
    public Season? CurrentSeason { get; set; }

    public GameEngine()
    {
    }

    public void CreateNewSeason(int year)
    {
        CurrentSeason = new Season(year);
    }
}