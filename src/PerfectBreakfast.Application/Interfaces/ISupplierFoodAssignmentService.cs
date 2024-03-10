using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface ISupplierFoodAssignmentService
    {
        public Task<OperationResult<List<SupplierFoodAssignmentResponse>>> CreateSupplierFoodAssignment(SupplierFoodAssignmentsRequest request);
        public Task<OperationResult<Pagination<SupplierFoodAssignmentForSupplier>>> GetSupplierFoodAssignmentBySupplier(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<Pagination<SupplierFoodAssignmentForPartner>>> GetSupplierFoodAssignmentByPartner(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<FoodAssignmentResponse>> ChangeStatusFoodAssignment(Guid id, int status);
        public Task<OperationResult<FoodAssignmentResponse>> CompleteFoodAssignment(Guid id);
        public Task<OperationResult<SupplierFoodAssignmentResponse>> UpdateSupplierFoodAssignment(UpdateSupplierFoodAssignment updateSupplierFoodAssignment);
    }
}
