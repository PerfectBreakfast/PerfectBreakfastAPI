namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentGroupByCompany
(
    string? CompanyName,
    List<FoodAssignmentResponse> FoodAssignmentResponses
);