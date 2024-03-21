using PerfectBreakfast.Application.Models.CompanyModels.Response;

namespace PerfectBreakfast.Application.Models.DailyOrder.Response;

public record DailyOrderForPartnerResponse(
    DateOnly CreationDate,
    DateOnly BookingDate,
    List<CompanyForDailyOrderResponse?> Companies);