namespace PBEMFootball.Common.Models;

public class Formation
{
    public Player? Goalkeeper { get; set; }
    public Player? Libero { get; set; }
    public List<Player> Defenders { get; set; } = new();
    public List<Player> Midfielders { get; set; } = new();
    public List<Player> Attackers { get; set; } = new();

    public (int po, int li, int di, int ce, int at) GetAreaTotals()
    {
        int po = Goalkeeper != null ? (Goalkeeper.Ability + Goalkeeper.Form) : 0;
        int li = Libero != null ? (Libero.Ability + Libero.Form) : 0;
        int di = 0;
        foreach (var p in Defenders)
            di += p.Ability + p.Form;
        int ce = 0;
        foreach (var p in Midfielders)
            ce += p.Ability + p.Form;
        int at = 0;
        foreach (var p in Attackers)
            at += p.Ability + p.Form;
        return (po, li, di, ce, at);
    }

    public int GetPlayerCount()
    {
        return (Goalkeeper != null ? 1 : 0) +
               (Libero != null ? 1 : 0) +
               Defenders.Count +
               Midfielders.Count +
               Attackers.Count;
    }
}