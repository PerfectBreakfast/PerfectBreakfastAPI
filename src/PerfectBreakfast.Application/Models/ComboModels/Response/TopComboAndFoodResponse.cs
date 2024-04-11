namespace PerfectBreakfast.Application.Models.ComboModels.Response;

public record TopComboAndFoodResponse
(
    Guid Id,
    string Type,
    string Name,
    string? Image,
    decimal Price,
    int Quantity,
    string Foods
    );