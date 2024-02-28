namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentGroupBySupplier
(
     string? SupplierName,
     List<PartnerFoodMealResponse> PartnerFoodMealResponses
     );