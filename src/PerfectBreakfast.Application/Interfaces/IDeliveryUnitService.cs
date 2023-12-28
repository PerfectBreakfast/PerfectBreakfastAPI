using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IDeliveryUnitService
{
    public Task<OperationResult<List<DeliveryUnitResponseModel>>> GetDeliveries();
}