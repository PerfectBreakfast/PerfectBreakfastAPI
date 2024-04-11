namespace PerfectBreakfast.Application.Models.CompanyModels.Response;

public record CompanyStatistic
(
    Guid? Id, 
    string? Name,
    string? PhoneNumber,
    string? Email,
    string? Address,
    int? OrderQuantity
    );