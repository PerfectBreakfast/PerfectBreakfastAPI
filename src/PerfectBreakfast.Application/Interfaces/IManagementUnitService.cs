using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Request;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;
using PerfectBreakfast.Application.Models.RoleModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IManagementUnitService
{
    public Task<OperationResult<List<ManagementUnitResponseModel>>> GetManagementUnits();
    public Task<OperationResult<ManagementUnitResponseModel>> GetManagementUnitId(Guid Id);
    public Task<OperationResult<List<RoleResponse>>> GetRoleByManagementUnit();
    public Task<OperationResult<ManagementUnitResponseModel>> CreateManagementUnit(CreateManagementUnitRequest requestModel);
    public Task<OperationResult<ManagementUnitResponseModel>> UpdateManagementUnit(Guid managementUnitId, UpdateManagementUnitRequest requestModel);
    public Task<OperationResult<ManagementUnitResponseModel>> RemoveManagementUnit(Guid managementUnitIdId);
}