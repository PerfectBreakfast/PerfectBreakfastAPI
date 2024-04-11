namespace PerfectBreakfast.Application.Models.OrderModel.Response;

public record OrderByDateStatistic
    (
        DateOnly Date,
        int OrderAmount
        );