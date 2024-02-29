using PerfectBreakfast.Application.Models.DaliyOrder.Response;

namespace PerfectBreakfast.Application.Models.CompanyModels.Response;

public record CompanyForDailyOrderResponse(
    Guid Id,
    string Name,
    string Address,
    List<DailyOrderModelResponse> DailyOrders
    );