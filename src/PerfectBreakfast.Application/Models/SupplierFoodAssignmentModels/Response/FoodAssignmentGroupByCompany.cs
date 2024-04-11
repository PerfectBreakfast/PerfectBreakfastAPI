namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentGroupByCompany
(
    Guid? Id,
    string? CompanyName,
    List<FoodAssignmentResponse> FoodAssignmentResponses
);