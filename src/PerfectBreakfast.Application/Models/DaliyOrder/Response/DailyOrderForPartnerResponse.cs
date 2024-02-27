using PerfectBreakfast.Application.Models.CompanyModels.Response;

namespace PerfectBreakfast.Application.Models.DaliyOrder.Response;

public record DailyOrderForPartnerResponse(
    DateOnly BookingDate,
    List<CompanyForDailyOrderResponse?> Companies);