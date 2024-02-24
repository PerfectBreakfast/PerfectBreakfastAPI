namespace PerfectBreakfast.Application.Interfaces;

public interface IManagementService
{
    public Task AutoUpdateAndCreateDailyOrder();
    public Task AutoCreateDailyOrderEachDay4PM();
}
