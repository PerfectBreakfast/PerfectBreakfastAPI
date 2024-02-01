namespace PerfectBreakfast.Application.Models.DaliyOrder.Response
{
    public record DailyOrderForPartnerResponse(
        DateOnly BookingDate,
        List<DailyOrderModelResponse> DailyOrderModelResponses);
}
