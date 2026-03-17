namespace PBEMFootball.Common.Models;

public class Season
{
    public int Year { get; set; }
    public List<Team> Teams { get; set; } = new();
}
