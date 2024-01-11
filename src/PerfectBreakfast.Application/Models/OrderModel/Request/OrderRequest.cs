namespace PerfectBreakfast.Application.Models.OrderModel.Request
{
    public record OrderRequest
    {
        public string Note { get; set; } = string.Empty;
        public string Payment { get; set; } = string.Empty;
        public List<OrderDetailRequest> OrderDetails { get; set; }
    }
}
