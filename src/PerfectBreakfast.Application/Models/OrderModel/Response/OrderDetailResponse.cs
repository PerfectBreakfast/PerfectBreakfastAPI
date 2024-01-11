namespace PerfectBreakfast.Application.Models.OrderModel.Response
{
    public record OrderDetailResponse
    {
        public Guid? ComboId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
