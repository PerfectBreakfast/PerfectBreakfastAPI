using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface ISupplierService
{
    public Task<OperationResult<List<SupplierResponse>>> GetSuppliers();
}