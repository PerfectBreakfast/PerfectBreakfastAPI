namespace PerfectBreakfast.Application.Models.UserModels.Request;

public record UpdateUserLoginGoogleRequest(
    string? PhoneNumber,
    Guid CompanyId
    );