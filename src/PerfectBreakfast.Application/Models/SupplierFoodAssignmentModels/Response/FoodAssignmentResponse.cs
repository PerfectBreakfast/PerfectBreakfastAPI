namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record FoodAssignmentResponse
    {
        public Guid Id { get; set; }
        public string? PartnerName { get; set; } = null;
        public string? FoodName { get; set; }
        public int AmountCooked { get; set; }
        public DateOnly DateCooked { get; set; }
        public decimal? ReceivedAmount { get; set; }
        public string Status { get; set; }
    }
}
