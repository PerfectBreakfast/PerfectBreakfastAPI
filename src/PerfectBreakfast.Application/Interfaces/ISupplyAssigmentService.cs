using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Request;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface ISupplyAssigmentService
{
    public Task<OperationResult<List<SupplyAssigmentResponse>>> GetSupplyAssigment();
    
    public Task<OperationResult<SupplyAssigmentResponse>> CreateSupplyAssigment(CreateSupplyAssigment requestModel);
    
}