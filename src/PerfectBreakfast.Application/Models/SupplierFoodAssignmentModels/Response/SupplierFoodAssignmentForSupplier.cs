namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record SupplierFoodAssignmentForSupplier(

     DateOnly? Date,
     List<FoodAssignmentGroupByPartner> FoodAssignmentGroupByPartners
);