using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.StatisticResponse;

public record FoodAssignmentGroupForSupplier
(
    string? SupplierName,
    List<FoodAssignmentResponse> FoodAssignmentResponses   
        
    );