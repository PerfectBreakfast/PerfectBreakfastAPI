namespace PerfectBreakfast.Application.Models.MealModels.Response;

public record MealResponse(
    Guid Id, 
    string MealType,
    TimeOnly? StartTime,
    TimeOnly? EndTime
    );