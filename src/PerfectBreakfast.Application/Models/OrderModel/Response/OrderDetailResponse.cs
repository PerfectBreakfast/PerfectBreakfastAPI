namespace PerfectBreakfast.Application.Models.OrderModel.Response
{
    public record OrderDetailResponse
    {
        public string? ComboName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Image { get; set; }
        public string? Foods { get; set; }
    }
}
