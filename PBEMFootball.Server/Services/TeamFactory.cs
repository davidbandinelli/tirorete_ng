namespace PBEMFootball.Server.Services;

using PBEMFootball.Common.Models;

public class TeamFactory
{
    public Team CreateInitialTeam(string teamName, string managerName)
    {
        var team = new Team(teamName, managerName);

        // 70 livelli totali: 34 per età I, 12 per età II, 12 per età III, 12 per età IV
        // Min 2, Max 12 per giocatore
        // Minimo 9 giocatori adulti

        // Età I: 34 punti su almeno 4 giocatori
        team.Players.Add(new Player("Marco Bianchi", PlayerPosition.Po, 10, PlayerAge.I) { Side = PlayerSide.D });
        team.Players.Add(new Player("Paolo Rossi", PlayerPosition.Di, 8, PlayerAge.I) { Side = PlayerSide.S });
        team.Players.Add(new Player("Luca Verdi", PlayerPosition.Di, 8, PlayerAge.I) { Side = PlayerSide.D });
        team.Players.Add(new Player("Andrea Neri", PlayerPosition.At, 8, PlayerAge.I) { Side = PlayerSide.SD }); // S+D

        // Età II: 12 punti su almeno 2 giocatori
        team.Players.Add(new Player("Stefano Blu", PlayerPosition.Di, 6, PlayerAge.II) { Side = PlayerSide.S });
        team.Players.Add(new Player("Davide Gialli", PlayerPosition.Ce, 6, PlayerAge.II) { Side = PlayerSide.D });

        // Età III: 12 punti su almeno 2 giocatori
        team.Players.Add(new Player("Giovanni Viola", PlayerPosition.Ce, 6, PlayerAge.III) { Side = PlayerSide.S });
        team.Players.Add(new Player("Francesco Arancio", PlayerPosition.At, 6, PlayerAge.III) { Side = PlayerSide.D });

        // Età IV: 12 punti su almeno 2 giocatori
        team.Players.Add(new Player("Roberto Grigio", PlayerPosition.At, 6, PlayerAge.IV) { Side = PlayerSide.SD }); // S+D
        team.Players.Add(new Player("Alessandro Rosa", PlayerPosition.Ce, 6, PlayerAge.IV) { Side = PlayerSide.S });

        // 3 Juniores con abilità 5 e forma +2
        for (int i = 0; i < 3; i++)
        {
            team.Players.Add(new Player($"Junior A{i + 1}", PlayerPosition.Ce, 5, PlayerAge.Juniores)
            {
                Form = 2,
                Side = PlayerSide.D
            });
        }

        // 3 Juniores con abilità 3 e forma +2
        for (int i = 0; i < 3; i++)
        {
            team.Players.Add(new Player($"Junior B{i + 1}", PlayerPosition.Di, 3, PlayerAge.Juniores)
            {
                Form = 2,
                Side = PlayerSide.S
            });
        }

        // 6 Primavera con abilità 2 e forma +2
        for (int i = 0; i < 6; i++)
        {
            team.Players.Add(new Player($"Primavera{i + 1}", PlayerPosition.Di, 2, PlayerAge.Primavera)
            {
                Form = 2,
                Side = i % 2 == 0 ? PlayerSide.D : PlayerSide.S
            });
        }

        // 1 panchinaro (allenatore o massaggiatore)
        team.Staff.Add(new BenchStaff("Allenatore", BenchStaffType.Coach));
        team.Money = 0;
        team.TrainingPoints = 5;
        team.GreatPerformancePoints = 30;

        return team;
    }
}