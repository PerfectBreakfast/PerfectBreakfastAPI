namespace PerfectBreakfast.Application.Models.MealModels.Response;

public record MealSubscriptionResponse(
    Guid Id, 
    string MealType,
    TimeOnly? StartTime,
    TimeOnly? EndTime
    );