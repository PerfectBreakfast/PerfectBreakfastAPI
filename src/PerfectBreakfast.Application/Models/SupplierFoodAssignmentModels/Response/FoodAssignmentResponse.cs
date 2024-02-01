namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record FoodAssignmentResponse
    {
        public string? FoodName { get; set; }
        public int AmountCooked { get; set; }
        public DateOnly DateCooked { get; set; }
        public decimal? ReceivedAmount { get; set; }
    }
}
