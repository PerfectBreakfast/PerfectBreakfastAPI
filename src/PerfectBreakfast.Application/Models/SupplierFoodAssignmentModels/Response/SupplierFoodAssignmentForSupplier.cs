namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record SupplierFoodAssignmentForSupplier(

     DateOnly? DateCooked,
     List<FoodAssignmentResponse> FoodAssignmentForSuppliers
);