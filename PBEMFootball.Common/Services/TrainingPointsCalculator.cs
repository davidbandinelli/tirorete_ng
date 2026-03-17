namespace PBEMFootball.Common.Services;

using PBEMFootball.Common.Models;

public class TrainingPointsCalculator
{
    public int CalculateSessionPoints(Team team, int wins, int draws)
    {
        int points = 0;
        int healthyAdults = 0;
        foreach (var player in team.Players)
        {
            if (player.Age != PlayerAge.Primavera && player.Age != PlayerAge.Juniores && player.Form >= 0)
            {
                healthyAdults++;
            }
        }
        points += Math.Min(healthyAdults, 20);
        points += wins * 10;
        points += draws * 5;
        return points;
    }
}