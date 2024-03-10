namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

public record UpdateSupplierFoodAssignment
(
    Guid? SupplierFoodAssignmentId,
    Guid? SupplierId
    );