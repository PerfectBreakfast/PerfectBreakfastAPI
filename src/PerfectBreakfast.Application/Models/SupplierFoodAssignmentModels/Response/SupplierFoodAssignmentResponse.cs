namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record SupplierFoodAssignmentResponse
    {
        public string? SupplierName { get; set; }
        public List<FoodAssignmentResponse> FoodAssignmentResponses { get; set; } = null;
    }
}
