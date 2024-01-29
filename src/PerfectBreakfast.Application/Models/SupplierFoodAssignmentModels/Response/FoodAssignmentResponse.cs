namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record FoodAssignmentResponse
    {
        public int AmountCooked { get; set; }
        public string? FoodName { get; set; }
    }
}
