namespace PerfectBreakfast.Application.Utils;

public static class Ramdom
{
    public static int GenerateCode()
    {
        long currentTick = DateTime.UtcNow.Ticks; 
        return (int)currentTick;
    }
}