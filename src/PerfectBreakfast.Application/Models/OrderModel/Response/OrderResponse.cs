using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.OrderModel.Response
{
    public record OrderResponse
    {
        public string Note { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int OrderCode { get; set; }
        public List<OrderDetailResponse> orderDetails { get; set; }

    }
}
