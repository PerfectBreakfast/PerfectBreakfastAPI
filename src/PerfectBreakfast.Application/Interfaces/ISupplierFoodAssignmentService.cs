using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface ISupplierFoodAssignmentService
    {
        public Task<OperationResult<List<SupplierFoodAssignmentResponse>>> CreateSupplierFoodAssignment(List<SupplierFoodAssignmentRequest> request);
        public Task<OperationResult<SupplierFoodAssignmentForSupplier>> GetSupplierFoodAssignmentBySupplier();
        public Task<OperationResult<Pagination<SupplierFoodAssignmentResponse>>> GetSupplierFoodAssignmentByPartner(int pageIndex = 0, int pageSize = 10);
    }
}
