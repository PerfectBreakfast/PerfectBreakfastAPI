namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record SupplierFoodAssignmentForSuperAdmin
    (
        DateOnly? CreationDate,
        DateOnly? BookingDate,
        List<FoodAssignmentGroupByPartner> FoodAssignmentGroupByPartners
        );