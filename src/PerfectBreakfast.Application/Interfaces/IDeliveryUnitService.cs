using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;
using PerfectBreakfast.Application.Models.RoleModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IDeliveryUnitService
{
    public Task<OperationResult<List<DeliveryUnitResponseModel>>> GetDeliveries();
    public Task<OperationResult<DeliveryUnitResponseModel>> GetDeliveryId(Guid deliveryId);
    public Task<OperationResult<List<RoleResponse>>> GetRoleByDeliveryUnit();
    public Task<OperationResult<DeliveryUnitResponseModel>> CreateDelivery(CreateDeliveryUnitRequest requestModel);
    public Task<OperationResult<DeliveryUnitResponseModel>> UpdateDelivery(Guid deliveryId, UpdateDeliveryUnitRequest requestModel);
    public Task<OperationResult<DeliveryUnitResponseModel>> RemoveDelivery(Guid deliveryId);
    public Task<OperationResult<Pagination<DeliveryUnitResponseModel>>> GetDeliveryUnitPaginationAsync(int pageIndex = 0, int pageSize = 10);
}