using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.OrderModel.Request
{
    public record UpdateOrderRequest
    {
        public OrderStatus OrderStatus { get; set; }
    }
}
