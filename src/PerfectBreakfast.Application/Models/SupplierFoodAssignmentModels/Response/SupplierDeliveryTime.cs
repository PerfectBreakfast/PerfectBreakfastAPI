namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record SupplierDeliveryTime
(
    TimeOnly? DeliveryTime,
    List<FoodAssignmentResponse> FoodAssignmentResponses
    );