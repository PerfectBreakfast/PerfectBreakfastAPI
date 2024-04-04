namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentGroupByPartner
(
    string? PartnerName,
    List<Guid?>? PackageIds,
    int? Status,
    List<FoodAssignmentResponse> FoodAssignmentResponses
    );