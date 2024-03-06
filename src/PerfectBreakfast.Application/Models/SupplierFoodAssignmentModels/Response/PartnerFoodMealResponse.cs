namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record PartnerFoodMealResponse
    (
        string Meal,
        List<FoodAssignmentResponse> FoodAssignmentResponses
        );
