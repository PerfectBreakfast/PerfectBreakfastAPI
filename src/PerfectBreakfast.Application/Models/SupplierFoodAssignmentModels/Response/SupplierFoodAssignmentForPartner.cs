namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record SupplierFoodAssignmentForPartner
(
    DateOnly? Date,
    List<PartnerFoodMealResponse> PartnerFoodMealResponses
);