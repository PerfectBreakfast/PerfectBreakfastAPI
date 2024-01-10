namespace PerfectBreakfast.Application.Models.UserModels.Request;

public record UpdateUserRequestModel
{
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}