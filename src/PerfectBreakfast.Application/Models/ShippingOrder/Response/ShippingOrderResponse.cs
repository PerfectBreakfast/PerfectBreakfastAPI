using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

public record ShippingOrderResponse()
{
    public Guid? DailyOrderId { get; set; }
    public Guid? ShipperId { get; set; }
    public ShippingStatus Status { get; set; }
}