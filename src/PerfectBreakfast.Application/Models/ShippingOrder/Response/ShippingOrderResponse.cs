namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

public record ShippingOrderResponse()
{
    public Guid? DailyOrderId { get; set; }
    public Guid? ShipperId { get; set; }
}