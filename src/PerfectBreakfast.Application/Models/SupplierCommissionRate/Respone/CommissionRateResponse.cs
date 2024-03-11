using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;

public record CommissionRateResponse(
    Guid Id,
    int CommissionRate,
    FoodResponse? Food
    );