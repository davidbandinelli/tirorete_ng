namespace PBEMFootball.Common.Models;

public class Stadium
{
    public int Level { get; set; }

    public int HomeFieldAdvantage
    {
        get
        {
            return Level switch
            {
                0 => 7,
                1 => 8,
                2 => 9,
                3 => 10,
                4 => 11,
                5 => 12,
                _ => 7
            };
        }
    }

    public int GetHomeEarnings()
    {
        return Level switch
        {
            1 => 10,
            2 => 20,
            3 => 30,
            4 => 40,
            5 => 50,
            _ => 0
        };
    }

    public int GetAwayEarnings()
    {
        return Level switch
        {
            1 => 5,
            2 => 10,
            3 => 15,
            4 => 20,
            5 => 25,
            _ => 0
        };
    }
}
