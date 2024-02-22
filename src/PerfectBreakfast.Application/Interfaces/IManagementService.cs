namespace PerfectBreakfast.Application.Interfaces;

public interface IManagementService
{
    public Task AutoUpdateDailyOrderAfter4PM();
    public Task AutoCreateDailyOrderEachDay4PM();
}
