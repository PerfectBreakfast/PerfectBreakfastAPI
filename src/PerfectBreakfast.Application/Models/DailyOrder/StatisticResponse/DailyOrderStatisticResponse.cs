namespace PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;

public record DailyOrderStatisticResponse(
    DateOnly CreationDate,
    DateOnly BookingDate,
    List<CompanyForDailyOrder?> CompanyForDailyOrders
    );