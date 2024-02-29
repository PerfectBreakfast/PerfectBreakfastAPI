using PerfectBreakfast.Application.Models.DailyOrder.Response;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

public record ShippingOrderHistoryForShipperResponse(
    Guid Id,
    string Status,
    DailyOrderModelResponse DailyOrder
);