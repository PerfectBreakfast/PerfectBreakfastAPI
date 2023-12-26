namespace PerfectBreakfast.Application.Models.UserModels.Response;

public sealed record UserResponse
{
    public string FullName { get; set; } 
    public string Email { get; set; } 
    public string Code { get; set; } 
    public string Password { get; set; } 
    public string PhoneNumber { get; set; } 
}