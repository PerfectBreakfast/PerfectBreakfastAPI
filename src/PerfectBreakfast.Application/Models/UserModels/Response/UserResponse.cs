namespace PerfectBreakfast.Application.Models.UserModels.Response;

public sealed record UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } 
    public string Name { get; set; } 
    public string Code { get; set; } 
    public string PhoneNumber { get; set; } 
    public bool EmailConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
}