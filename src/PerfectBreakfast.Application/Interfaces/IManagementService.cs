namespace PerfectBreakfast.Application.Interfaces;

public interface IManagementService
{
    public Task AutoUpdateAndCreateDailyOrderAfter4PM();
    public Task AutoCreateDailyOrderEachDay4PM();
}
