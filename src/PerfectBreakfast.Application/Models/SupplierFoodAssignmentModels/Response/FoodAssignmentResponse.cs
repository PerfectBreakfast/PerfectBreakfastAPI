namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record FoodAssignmentResponse
    {
        public string? PartnerName { get; set; } = null;
        public string? SupplierName { get; set; } = null;
        public string? FoodName { get; set; }
        public int AmountCooked { get; set; }
        public DateOnly DateCooked { get; set; }
        public decimal? ReceivedAmount { get; set; }
    }
}
