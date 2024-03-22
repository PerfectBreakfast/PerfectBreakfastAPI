using Microsoft.AspNetCore.Http;

namespace PerfectBreakfast.Application.Models.UserModels.Request;

public record UpdateProfileModel
    (
        string? Name,
        string? PhoneNumber,
        IFormFile? Image,
        Guid? CompanyId
        );