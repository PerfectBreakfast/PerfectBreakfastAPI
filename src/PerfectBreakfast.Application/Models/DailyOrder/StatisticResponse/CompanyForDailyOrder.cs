namespace PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;

public record CompanyForDailyOrder
(
    string? Name,
    string? Delivery,
    string? Partner,
    List<MealForDailyOrder?> MealForDailyOrders
    );