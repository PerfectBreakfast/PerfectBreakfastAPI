using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;


namespace PerfectBreakfast.Application.Interfaces;

public interface ISupplierCommissionRateService
{
    public Task<OperationResult<List<SupplierCommissionRateRespone>>> GetSupplierCommissionRates();
    public Task<OperationResult<SupplierCommissionRateRespone>> GetSupplierCommissionRateId(Guid id);
    //public Task<OperationResult<SupplierMoreFoodRespone>> GetSupplierMoreFood(Guid supplierId);
    public Task<OperationResult<List<SupplierCommissionRateRespone>>> CreateSupplierCommissionRate(CreateSupplierMoreFood createSupplierCommissionRateRequest);
    public Task<OperationResult<SupplierCommissionRateRespone>> DeleteCSupplierCommissionRate(Guid id);
    public Task<OperationResult<SupplierCommissionRateRespone>> UpdateSupplierCommissionRate(Guid id, UpdateSupplierCommissionRateRequest supplierCommissionRateRequest);
    public Task<OperationResult<Pagination<SupplierCommissionRateRespone>>> GetSupplierCommissionRatePaginationAsync(int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<SupplierCommissionRateRespone>> Delete(Guid id);
    
}