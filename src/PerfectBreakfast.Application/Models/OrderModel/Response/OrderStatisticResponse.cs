namespace PerfectBreakfast.Application.Models.OrderModel.Response;

public record OrderStatisticResponse(
    DateOnly FromDate,
    DateOnly ToDate,
    int TotalAmount,
    int CompleteAmount,
    string PopularCombo
    );