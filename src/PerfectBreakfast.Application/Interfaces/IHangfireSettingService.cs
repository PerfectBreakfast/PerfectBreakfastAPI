using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SettingModels.Request;

namespace PerfectBreakfast.Application.Interfaces;

public interface IHangfireSettingService
{
    public Task<OperationResult<bool>> UpdateTime(TimeSettingRequest request);
}