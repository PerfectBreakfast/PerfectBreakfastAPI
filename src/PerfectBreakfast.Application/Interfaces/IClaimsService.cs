namespace PerfectBreakfast.Application.Interfaces;

public interface IClaimsService
{
    public Guid GetCurrentUserId { get; }
}