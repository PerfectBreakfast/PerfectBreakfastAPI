using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IDeliveryService
{
    public Task<OperationResult<List<DeliveryResponseModel>>> GetDeliveries();
    public Task<OperationResult<DeliveryDetailResponse>> GetDeliveryId(Guid deliveryId);
    public Task<OperationResult<DeliveryResponseModel>> CreateDelivery(CreateDeliveryRequest requestModel);
    public Task<OperationResult<DeliveryResponseModel>> UpdateDelivery(Guid deliveryId, UpdateDeliveryRequest requestModel);
    public Task<OperationResult<DeliveryResponseModel>> RemoveDelivery(Guid deliveryId);
    public Task<OperationResult<Pagination<DeliveryResponseModel>>> GetDeliveryUnitPaginationAsync(string? searchTerm,int pageIndex = 0, int pageSize = 10);
}