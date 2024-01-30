namespace PerfectBreakfast.Application.Models.DaliyOrder.Response;

public record DailyOrderForDeliveryUnitResponse(
    DateOnly BookingDate,
    List<DailyOrderModelResponse> DailyOrderModelResponses);