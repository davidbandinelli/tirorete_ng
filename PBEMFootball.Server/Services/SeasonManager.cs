namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;

public class SeasonManager
{
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