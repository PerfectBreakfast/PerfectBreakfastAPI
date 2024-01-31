namespace PerfectBreakfast.Application.Models.DaliyOrder.Response;

public record DailyOrderForDeliveryResponse(
    DateOnly BookingDate,
    List<DailyOrderModelResponse> DailyOrderModelResponses);