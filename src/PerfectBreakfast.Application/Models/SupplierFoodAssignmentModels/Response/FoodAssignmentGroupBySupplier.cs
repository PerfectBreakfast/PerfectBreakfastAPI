namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentGroupBySupplier
(
    Guid? SupplierId,
    string? SupplierName, 
    List<Guid?> PackageIds,
    int? Status,
    List<FoodAssignmentResponse> FoodAssignmentResponses
     );