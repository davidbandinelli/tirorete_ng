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

    public Team CreateRandomTeam(int teamNumber, Random random)
    {
        // Nomi casuali italiani per squadre
        string[] cityNames = { "Milano", "Roma", "Napoli", "Torino", "Firenze", "Bologna", "Genova", "Verona", 
                               "Palermo", "Venezia", "Brescia", "Parma", "Cagliari", "Lecce", "Bari", "Perugia" };
        string[] suffixes = { "FC", "Calcio", "United", "AC", "SC", "US", "AS" };

        string teamName = $"{cityNames[random.Next(cityNames.Length)]} {suffixes[random.Next(suffixes.Length)]} {teamNumber}";

        // Nomi casuali per manager
        string[] firstNames = { "Mario", "Luigi", "Giovanni", "Antonio", "Francesco", "Giuseppe", "Marco", "Alessandro",
                                "Stefano", "Roberto", "Carlo", "Andrea", "Paolo", "Luca", "Fabio", "Davide" };
        string[] lastNames = { "Rossi", "Bianchi", "Verdi", "Neri", "Russo", "Ferrari", "Esposito", "Romano",
                               "Colombo", "Ricci", "Marino", "Greco", "Bruno", "Gallo", "Conti", "De Luca" };

        string managerName = $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";

        var team = new Team(teamName, managerName);

        // Lista di nomi casuali per giocatori
        string[] playerFirstNames = { "Marco", "Luca", "Andrea", "Paolo", "Giovanni", "Francesco", "Stefano", 
                                      "Roberto", "Alessandro", "Davide", "Matteo", "Simone", "Federico", "Lorenzo",
                                      "Gabriele", "Riccardo", "Tommaso", "Nicola", "Antonio", "Giuseppe" };
        string[] playerLastNames = { "Rossi", "Bianchi", "Verdi", "Neri", "Russo", "Ferrari", "Esposito", 
                                     "Romano", "Colombo", "Ricci", "Marino", "Greco", "Bruno", "Gallo",
                                     "Conti", "De Luca", "Costa", "Fontana", "Serra", "Villa" };

        // 70 livelli totali: 34 per età I, 12 per età II, 12 per età III, 12 per età IV
        // Distribuzione casuale rispettando i vincoli

        // Età I: 34 punti distribuiti casualmente (min 2, max 12, minimo 4 giocatori adulti)
        int pointsToDistribute = 34;
        int minPlayers = 4;
        int maxPlayers = 8; // Per avere varietà
        int playersCount = random.Next(minPlayers, maxPlayers + 1);
        var ageIAbilities = DistributePointsRandomly(pointsToDistribute, playersCount, 2, 12, random);

        var positions = new[] { PlayerPosition.Po, PlayerPosition.Di, PlayerPosition.Di, PlayerPosition.Ce, 
                               PlayerPosition.At, PlayerPosition.Di, PlayerPosition.Ce, PlayerPosition.At };
        var sides = new[] { PlayerSide.D, PlayerSide.S, PlayerSide.D, PlayerSide.S, PlayerSide.D, PlayerSide.S, PlayerSide.D, PlayerSide.S };

        for (int i = 0; i < ageIAbilities.Count; i++)
        {
            string name = $"{playerFirstNames[random.Next(playerFirstNames.Length)]} {playerLastNames[random.Next(playerLastNames.Length)]}";
            var side = i < 2 && random.Next(100) < 20 ? PlayerSide.SD : sides[i % sides.Length]; // 20% chance di S+D nei primi 2
            team.Players.Add(new Player(name, positions[i % positions.Length], ageIAbilities[i], PlayerAge.I) { Side = side });
        }

        // Età II: 12 punti
        var ageIIAbilities = DistributePointsRandomly(12, random.Next(2, 4), 2, 12, random);
        foreach (var ability in ageIIAbilities)
        {
            string name = $"{playerFirstNames[random.Next(playerFirstNames.Length)]} {playerLastNames[random.Next(playerLastNames.Length)]}";
            var position = positions[random.Next(1, positions.Length)]; // Esclude portiere
            var side = sides[random.Next(sides.Length)];
            team.Players.Add(new Player(name, position, ability, PlayerAge.II) { Side = side });
        }

        // Età III: 12 punti
        var ageIIIAbilities = DistributePointsRandomly(12, random.Next(2, 4), 2, 12, random);
        foreach (var ability in ageIIIAbilities)
        {
            string name = $"{playerFirstNames[random.Next(playerFirstNames.Length)]} {playerLastNames[random.Next(playerLastNames.Length)]}";
            var position = positions[random.Next(1, positions.Length)];
            var side = sides[random.Next(sides.Length)];
            team.Players.Add(new Player(name, position, ability, PlayerAge.III) { Side = side });
        }

        // Età IV: 12 punti
        var ageIVAbilities = DistributePointsRandomly(12, random.Next(2, 4), 2, 12, random);
        foreach (var ability in ageIVAbilities)
        {
            string name = $"{playerFirstNames[random.Next(playerFirstNames.Length)]} {playerLastNames[random.Next(playerLastNames.Length)]}";
            var position = positions[random.Next(1, positions.Length)];
            var side = sides[random.Next(sides.Length)];
            team.Players.Add(new Player(name, position, ability, PlayerAge.IV) { Side = side });
        }

        // 3 Juniores con abilità 5 e forma +2
        for (int i = 0; i < 3; i++)
        {
            string name = $"{playerFirstNames[random.Next(playerFirstNames.Length)]} {playerLastNames[random.Next(playerLastNames.Length)]}";
            team.Players.Add(new Player(name, positions[random.Next(1, positions.Length)], 5, PlayerAge.Juniores)
            {
                Form = 2,
                Side = sides[random.Next(sides.Length)]
            });
        }

        // 3 Juniores con abilità 3 e forma +2
        for (int i = 0; i < 3; i++)
        {
            string name = $"{playerFirstNames[random.Next(playerFirstNames.Length)]} {playerLastNames[random.Next(playerLastNames.Length)]}";
            team.Players.Add(new Player(name, positions[random.Next(1, positions.Length)], 3, PlayerAge.Juniores)
            {
                Form = 2,
                Side = sides[random.Next(sides.Length)]
            });
        }

        // 6 Primavera con abilità 2 e forma +2
        for (int i = 0; i < 6; i++)
        {
            string name = $"{playerFirstNames[random.Next(playerFirstNames.Length)]} {playerLastNames[random.Next(playerLastNames.Length)]}";
            team.Players.Add(new Player(name, positions[random.Next(1, positions.Length)], 2, PlayerAge.Primavera)
            {
                Form = 2,
                Side = sides[random.Next(sides.Length)]
            });
        }

        // Staff casuale
        int coachCount = random.Next(1, 3); // 1-2 allenatori
        for (int i = 0; i < coachCount; i++)
        {
            team.Staff.Add(new BenchStaff($"Allenatore {i + 1}", BenchStaffType.Coach));
        }

        if (random.Next(100) < 50) // 50% possibilità di avere un massaggiatore
        {
            team.Staff.Add(new BenchStaff("Massaggiatore", BenchStaffType.Masseur));
        }

        if (random.Next(100) < 30) // 30% possibilità di avere uno scout
        {
            team.Staff.Add(new BenchStaff("Scout", BenchStaffType.Scout));
        }

        if (random.Next(100) < 40) // 40% possibilità di avere un tattico
        {
            team.Staff.Add(new BenchStaff("Tattico", BenchStaffType.Tactician));
        }

        // Risorse casuali
        team.Money = random.Next(0, 100);
        team.TrainingPoints = random.Next(0, 20);
        team.GreatPerformancePoints = 30; // Fisso per regolamento: 30 PGP per stagione (non trasferibili)
        team.SpecialPoints = random.Next(0, 10);

        return team;
    }

    private List<int> DistributePointsRandomly(int totalPoints, int playerCount, int minAbility, int maxAbility, Random random)
    {
        var abilities = new List<int>();

        // Inizializza tutti al minimo
        for (int i = 0; i < playerCount; i++)
        {
            abilities.Add(minAbility);
        }

        int remainingPoints = totalPoints - (playerCount * minAbility);

        // Distribuisci i punti rimanenti casualmente
        while (remainingPoints > 0)
        {
            int playerIndex = random.Next(playerCount);
            if (abilities[playerIndex] < maxAbility)
            {
                abilities[playerIndex]++;
                remainingPoints--;
            }
        }

        return abilities;
    }
}