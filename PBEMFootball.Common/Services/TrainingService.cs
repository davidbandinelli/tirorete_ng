namespace PBEMFootball.Common.Services;

using PBEMFootball.Common.Models;

public class TrainingService
{
    public int CalculateFormTrainingCost(Player player)
    {
        if (player.Age == PlayerAge.Primavera || player.Age == PlayerAge.Juniores)
            return 1;
        return (int)player.Age;
    }

    public bool CanTrainAbility(Player player)
    {
        if (player.Age == PlayerAge.Primavera || player.Age == PlayerAge.Juniores)
            return false;
        return true;
    }
}