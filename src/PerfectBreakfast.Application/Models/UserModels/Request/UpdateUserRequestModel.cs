using Microsoft.AspNetCore.Http;

namespace PerfectBreakfast.Application.Models.UserModels.Request;

public record UpdateUserRequestModel
{
    public string? Name { get; set; } 
    public string? PhoneNumber { get; set; } 
    public IFormFile? Image { get; set; }
}