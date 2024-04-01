namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

public record FoodAssignmentByPartnerRequest
    (
        Guid? SupplierId,
        List<Guid?> PackageIds
        );