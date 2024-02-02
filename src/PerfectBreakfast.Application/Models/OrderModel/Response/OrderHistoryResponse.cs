namespace PerfectBreakfast.Application.Models.OrderModel.Response;

public sealed record OrderHistoryResponse(
    Guid Id,
    string Note,
    decimal TotalPrice,
    string OrderStatus,
    int OrderCode,
    DateTime CreationDate,
    int ComboCount,
    string CompanyName);