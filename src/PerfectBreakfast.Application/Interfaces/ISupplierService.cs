using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface ISupplierService
{
    public Task<OperationResult<List<SupplierResponse>>> GetSuppliers();
    public Task<OperationResult<SupplierResponse>> GetSupplierId(Guid Id);
    public Task<OperationResult<SupplierResponse>> CreateSupplier(CreateSupplierRequestModel requestModel);
    public Task<OperationResult<SupplierResponse>> UpdateSupplier(Guid supplierId,UpdateSupplierRequestModel requestModel);
    public Task<OperationResult<Pagination<SupplierResponse>>> GetPaginationAsync(int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<SupplierResponse>> RemoveSupplier(Guid supplierId);
}