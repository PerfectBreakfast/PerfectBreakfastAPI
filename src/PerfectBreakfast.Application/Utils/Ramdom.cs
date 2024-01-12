namespace PerfectBreakfast.Application.Utils;

public static class Random
{
    public static int GenerateCode()
    {
        long currentTick = DateTime.UtcNow.Ticks; 
        byte[] bytes = BitConverter.GetBytes(currentTick);
        int orderCode = BitConverter.ToInt32(bytes, 0);
        return orderCode;
    }
}