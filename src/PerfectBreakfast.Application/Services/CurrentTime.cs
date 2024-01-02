using PerfectBreakfast.Application.Interfaces;

namespace PerfectBreakfast.Application.Services;

public class CurrentTime : ICurrentTime
{
    public DateTime GetCurrentTime() => DateTime.UtcNow.AddHours(7);
}