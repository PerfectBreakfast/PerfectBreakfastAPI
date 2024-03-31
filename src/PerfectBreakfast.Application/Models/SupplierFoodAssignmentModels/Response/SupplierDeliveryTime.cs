namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record SupplierDeliveryTime
(
    string? DeliveryTime,
    List<FoodAssignmentGroupByPartner> FoodAssignmentGroupByPartners
        );