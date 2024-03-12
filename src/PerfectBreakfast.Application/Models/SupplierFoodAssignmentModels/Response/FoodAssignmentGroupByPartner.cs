namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentGroupByPartner
(
    string PartnerName,
    List<SupplierDeliveryTime> SupplierDeliveryTimes
    );