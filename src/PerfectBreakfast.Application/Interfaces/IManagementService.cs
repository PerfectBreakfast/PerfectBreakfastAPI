namespace PerfectBreakfast.Application.Interfaces;

public interface IManagementService
{
    public Task AutoUpdateAndCreateDailyOrder();
    public Task AutoCancelOrderWhenOverTime(Guid orderId);
}
