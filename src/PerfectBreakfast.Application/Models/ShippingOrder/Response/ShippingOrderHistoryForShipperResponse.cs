using PerfectBreakfast.Application.Models.DaliyOrder.Response;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

public record ShippingOrderHistoryForShipperResponse(
    Guid Id,
    string Status,
    DailyOrderModelResponse DailyOrder
);