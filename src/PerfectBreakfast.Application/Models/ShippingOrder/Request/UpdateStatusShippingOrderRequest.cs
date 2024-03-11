using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Request;

public record UpdateStatusShippingOrderRequest()
{
    public ShippingStatus Status { get; set; }
}