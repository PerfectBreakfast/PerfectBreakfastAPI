namespace PerfectBreakfast.Application.Models.PayOSModels.Response;

public record Response(
    int error,
    string message,
    object? data
);