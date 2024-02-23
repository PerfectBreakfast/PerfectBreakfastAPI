﻿using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface ISupplierFoodAssignmentService
    {
        public Task<OperationResult<List<SupplierFoodAssignmentResponse>>> CreateSupplierFoodAssignment(List<SupplierFoodAssignmentRequest> request);
        public Task<OperationResult<Pagination<SupplierFoodAssignmentForSupplier>>> GetSupplierFoodAssignmentBySupplier(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<Pagination<SupplierFoodAssignmetForPartner>>> GetSupplierFoodAssignmentByPartner(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<FoodAssignmentResponse>> ConfirmFoodAssignment(Guid id);
        public Task<OperationResult<FoodAssignmentResponse>> CompleteFoodAssignment(Guid id);
    }
}
