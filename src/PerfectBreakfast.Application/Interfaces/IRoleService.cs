using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
namespace PerfectBreakfast.Application.Interfaces;

public interface IRoleService
{
    public Task<OperationResult<List<RoleResponse>>> GetManagementRole();
    public Task<OperationResult<RoleResponse>> GetRoleById(Guid id);
    public Task<OperationResult<List<RoleResponse>>> GetRoleByUnitId(Guid unitId);
    public Task<OperationResult<bool>> CreateRole(CreatRoleRequest requestModel);
    public Task<OperationResult<bool>> UpdateRole(Guid roleId, UpdateRolerequest requestModel);
    public Task<OperationResult<bool>> RemoveRole(Guid roleId);
}
