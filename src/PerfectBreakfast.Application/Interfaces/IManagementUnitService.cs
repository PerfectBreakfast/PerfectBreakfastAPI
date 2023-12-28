using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;

namespace PerfectBreakfast.Application.Interfaces;

public interface IManagementUnitService
{
    public Task<OperationResult<List<ManagementUnitResponseModel>>> GetManagementUnits();
}