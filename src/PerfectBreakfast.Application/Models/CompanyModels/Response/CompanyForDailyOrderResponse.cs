using PerfectBreakfast.Application.Models.DailyOrder.Response;

namespace PerfectBreakfast.Application.Models.CompanyModels.Response;

public record CompanyForDailyOrderResponse(
    Guid Id,
    string Name,
    string Address,
    string? Delivery,
    string? Partner,
    List<DailyOrderModelResponse> DailyOrders
    );