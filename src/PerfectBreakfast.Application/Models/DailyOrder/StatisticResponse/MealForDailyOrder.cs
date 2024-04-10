namespace PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;

public record MealForDailyOrder
(
    string? Meal,
    string? Time,
    int TotalOrderQuantity,
    int CompleteQuantity,
    int CancelQuantity,
    decimal? TotalPrice,
    List<FoodForDailyOrder?> FoodForDailyOrders
        );