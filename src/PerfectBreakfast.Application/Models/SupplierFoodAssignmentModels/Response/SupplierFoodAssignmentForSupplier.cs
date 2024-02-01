namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record SupplierFoodAssignmentForSupplier
{
    public string? PartnerName { get; set; }
    public List<FoodAssignmentResponse> FoodAssignmentResponses { get; set; } = null;
}