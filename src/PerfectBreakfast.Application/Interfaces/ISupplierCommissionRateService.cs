using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;


namespace PerfectBreakfast.Application.Interfaces;

public interface ISupplierCommissionRateService
{
    public Task<OperationResult<SupplierCommissionRateRespone>> GetSupplierCommissionRateId(Guid id);
    public Task<OperationResult<List<FoodResponse>>> GetSupplierMoreFood(Guid supplierId);
    public Task<OperationResult<List<SupplierCommissionRateRespone>>> CreateSupplierCommissionRate(CreateSupplierCommissionRateRequest request);
    public Task<OperationResult<SupplierCommissionRateRespone>> DeleteCSupplierCommissionRate(Guid id);
    public Task<OperationResult<SupplierCommissionRateRespone>> UpdateSupplierCommissionRate(Guid id, UpdateSupplierCommissionRateRequest supplierCommissionRateRequest);
    public Task<OperationResult<SupplierCommissionRateRespone>> Delete(Guid id);
    
}