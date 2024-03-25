namespace PerfectBreakfast.Application.Models.ShippingOrder.Request;

public sealed record CreateShippingOrderRequest(Guid DailyOrderId, List<Guid?> ShipperIds);
