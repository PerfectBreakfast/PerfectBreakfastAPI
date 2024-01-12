using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IDeliveryUnitService
{
    public Task<OperationResult<List<DeliveryUnitResponseModel>>> GetDeliveries();
    public Task<OperationResult<DeliveryUnitResponseModel>> GetDeliveryId(Guid deliveryId);
    public Task<OperationResult<DeliveryUnitResponseModel>> CreateDelivery(CreateDeliveryUnitRequest requestModel);
    public Task<OperationResult<DeliveryUnitResponseModel>> UpdateDelivery(Guid deliveryId, UpdateDeliveryUnitRequest requestModel);
    public Task<OperationResult<DeliveryUnitResponseModel>> RemoveDelivery(Guid deliveryId);
}