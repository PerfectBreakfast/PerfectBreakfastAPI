namespace PerfectBreakfast.Application.Models.MealModels.Request;

public record MealModel(
    Guid MealId,
    TimeOnly StartTime,
    TimeOnly EndTime
    );