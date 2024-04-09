namespace PerfectBreakfast.Application.Models.OrderModel.Response;

public record OrderStatisticResponse(
    DateOnly? FromDate,
    DateOnly? ToDate,
    int TotalAmount,
    int CompleteAmount,
    double CompletePercent,
    int CancelledForOverdueAmount,
    double CancelledForOverduePercent
    );