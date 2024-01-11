namespace PerfectBreakfast.Application.Models.OrderModel.Response
{
    public record OrderDetailResponse
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
