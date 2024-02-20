using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

public record ShippingOrderForShipperResponse(
    Guid Id,
    string Status
);