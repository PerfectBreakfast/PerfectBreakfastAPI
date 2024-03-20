using Hangfire;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SettingModels.Request;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class HangFireSettingService : IHangfireSettingService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IUnitOfWork _unitOfWork;
    
    public HangFireSettingService(IRecurringJobManager recurringJobManager,IUnitOfWork unitOfWork)
    {
        _recurringJobManager = recurringJobManager;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<OperationResult<TimeOnly>> UpdateTime(Guid id,TimeSettingRequest request)
    {
        var result = new OperationResult<TimeOnly>();
        try
        {
            var setting = await _unitOfWork.SettingRepository.GetByIdAsync(id);
            TimeOnly adjustedTime = request.Time.AddHours(-7); // Giả sử bạn cần điều chỉnh thời gian
            // Tạo cron schedule
            var cronSchedule =
                $"{adjustedTime.Minute} {adjustedTime.Hour} * * *"; // Chạy hàng ngày vào thời gian cụ thể
            _recurringJobManager.AddOrUpdate<IManagementService>("recurringJob1",
                d => d.AutoUpdateAndCreateDailyOrder(), cronSchedule);
            setting.Time = request.Time;
            _unitOfWork.SettingRepository.Update(setting);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = request.Time;
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound,"Không tìm thấy ID");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Setting>> GetTimeAuto()
    {
        var result = new OperationResult<Setting>();
        try
        {
            var setting = await _unitOfWork.SettingRepository.GetAllAsync();
            result.Payload = setting.First();
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}