namespace PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;

public record MealForDailyOrder
(
    string? Meal,
    TimeOnly TimeHandover,
    List<FoodForDailyOrder?> FoodForDailyOrders
        );