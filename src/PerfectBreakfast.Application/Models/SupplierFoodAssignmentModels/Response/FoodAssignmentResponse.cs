namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record FoodAssignmentResponse
    {
        public Guid Id { get; set; }
        public Guid? DailyOrderId { get; set; }
        public string? PartnerName { get; set; } = null;
        public Guid? FoodId { get; set; }
        public string? FoodName { get; set; }
        public int AmountCooked { get; set; }
        public decimal? ReceivedAmount { get; set; }
        public decimal? CommissionRate { get; set; }
        public string Status { get; set; }
    }
}
