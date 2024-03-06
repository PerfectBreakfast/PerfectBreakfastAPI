using PerfectBreakfast.Application.Models.CompanyModels.Response;

namespace PerfectBreakfast.Application.Models.DailyOrder.Response;

public record DailyOrderForDeliveryResponse(
    DateOnly BookingDate,
    List<CompanyForDailyOrderResponse?> Companies);