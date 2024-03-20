using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SettingModels.Request;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface IHangfireSettingService
{
    public Task<OperationResult<TimeOnly>> UpdateTime(Guid id, TimeSettingRequest request);
    public Task<OperationResult<Setting>> GetTimeAuto();
}