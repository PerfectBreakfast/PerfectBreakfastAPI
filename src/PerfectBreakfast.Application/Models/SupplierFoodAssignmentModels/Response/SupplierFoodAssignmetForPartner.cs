namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record SupplierFoodAssignmetForPartner
(
    DateOnly? DateCooked,
    List<FoodAssignmentResponse> FoodAssignmentForSuppliers
);