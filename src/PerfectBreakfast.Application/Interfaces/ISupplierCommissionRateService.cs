using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ComboModels.Request;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;
using PerfectBreakfast.Application.Models.SupplierModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface ISupplierCommissionRateService
{
    public Task<OperationResult<List<SupplierCommissionRateRespone>>> GetSupplierCommissionRates();
    public Task<OperationResult<SupplierCommissionRateRespone>> GetSupplierCommissionRateId(Guid id);
    public Task<OperationResult<SupplierCommissionRateRespone>> CreateSupplierCommissionRate(CreateSupplierCommissionRateRequest createSupplierCommissionRateRequest);
    public Task<OperationResult<SupplierCommissionRateRespone>> DeleteCSupplierCommissionRate(Guid id);
    public Task<OperationResult<SupplierCommissionRateRespone>> UpdateSupplierCommissionRate(Guid id, UpdateSupplierCommissionRateRequest supplierCommissionRateRequest);
    public Task<OperationResult<Pagination<SupplierCommissionRateRespone>>> GetSupplierCommissionRatePaginationAsync(int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<SupplierCommissionRateRespone>> Delete(Guid id);
}