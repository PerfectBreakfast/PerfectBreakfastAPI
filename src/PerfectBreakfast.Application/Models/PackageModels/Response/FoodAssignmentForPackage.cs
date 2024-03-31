namespace PerfectBreakfast.Application.Models.PackageModels.Response;

public record FoodAssignmentForPackage
(
    Guid Id,
    Guid? FoodId,
    string? FoodName,
    int AmountCooked,
    string Status,
    DateTime? DeliveryDeadline,
    string? SupplierName
    );