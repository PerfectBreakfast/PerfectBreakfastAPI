using Hangfire;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SettingModels.Request;

namespace PerfectBreakfast.Application.Services;

public class HangFireSettingService : IHangfireSettingService
{
    private readonly IRecurringJobManager _recurringJobManager;
    
    public HangFireSettingService(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }
    
    public async Task<OperationResult<bool>> UpdateTime(TimeSettingRequest request)
    {
        var result = new OperationResult<bool>();
        try
        {
            var time = request.Time.Subtract(TimeSpan.FromHours(7));
            var cronSchedule = $"{time.Minute} {time.Hour} * * * ";  // parse qua dang chạy hàng ngày vào lúc ....
            _recurringJobManager.AddOrUpdate<IManagementService>("recurringJob3", 
                d => d.AutoUpdateAndCreateDailyOrder(), cronSchedule);
            result.Payload = true;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}