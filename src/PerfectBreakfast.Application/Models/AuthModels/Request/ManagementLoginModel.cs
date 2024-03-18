namespace PerfectBreakfast.Application.Models.AuthModels.Request;

public record ManagementLoginModel(
    string Email,
    string Password,
    Guid RoleId 
    );