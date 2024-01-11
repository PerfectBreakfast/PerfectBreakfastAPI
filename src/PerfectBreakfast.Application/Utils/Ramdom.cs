namespace PerfectBreakfast.Application.Utils;

public static class Random
{
    public static int GenerateCode()
    {
        long currentTick = DateTime.UtcNow.Ticks; 
        return (int)currentTick;
    }
}