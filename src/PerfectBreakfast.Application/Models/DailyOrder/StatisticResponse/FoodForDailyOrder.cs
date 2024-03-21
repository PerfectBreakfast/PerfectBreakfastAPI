namespace PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;

public record FoodForDailyOrder
(
    string? Name,
    string? Food,
    int? Quantity
    );