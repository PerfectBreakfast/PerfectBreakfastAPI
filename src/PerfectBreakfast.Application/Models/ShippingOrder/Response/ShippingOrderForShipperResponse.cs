using PerfectBreakfast.Application.Models.DailyOrder.Response;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

public record ShippingOrderForShipperResponse(
    Guid Id,
    string Status,
    DailyOrderDto DailyOrder
);