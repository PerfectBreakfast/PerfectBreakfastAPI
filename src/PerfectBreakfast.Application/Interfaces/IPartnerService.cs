using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.PartnerModels.Request;
using PerfectBreakfast.Application.Models.PartnerModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IPartnerService
{
    public Task<OperationResult<List<ManagementUnitResponseModel>>> GetManagementUnits();
    public Task<OperationResult<ManagementUnitDetailResponse>> GetManagementUnitId(Guid id);
    public Task<OperationResult<ManagementUnitResponseModel>> CreateManagementUnit(CreateManagementUnitRequest requestModel);
    public Task<OperationResult<ManagementUnitResponseModel>> UpdateManagementUnit(Guid managementUnitId, UpdateManagementUnitRequest requestModel);
    public Task<OperationResult<ManagementUnitResponseModel>> RemoveManagementUnit(Guid managementUnitIdId);
    public Task<OperationResult<Pagination<ManagementUnitResponseModel>>> GetManagementUnitPaginationAsync(string? searchTerm,int pageIndex = 0, int pageSize = 10);
}