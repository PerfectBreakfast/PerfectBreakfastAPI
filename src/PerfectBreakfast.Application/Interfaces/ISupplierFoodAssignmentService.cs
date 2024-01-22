using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface ISupplierFoodAssignmentService
    {
        public Task<OperationResult<List<SupplierFoodAssignmentResponse>>> CreateSupplierFoodAssignment(SupplierFoodAssignmentRequest request);
        public Task<OperationResult<List<SupplierFoodAssignmentResponse>>> GetSupplierFoodAssignment(Guid id);
    }
}
