

using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Request;

public record class CreateShippingOrderRequest
{
    public ShippingStatus Status { get; set; }
    public Guid? DailyOrderId { get; set; }
    public Guid? ShipperId { get; set; }
}
