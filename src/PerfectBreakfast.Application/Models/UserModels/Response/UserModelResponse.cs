namespace PerfectBreakfast.Application.Models.UserModels.Response;

public record UserModelResponse(
    Guid? Id,
    string? Email,
    string? Name,
    string? Image,
    string? Code,
    string? PhoneNumber,
    DateTime CreationDate,
    bool LockoutEnabled,
    List<string> Roles
    );
    