namespace PerfectBreakfast.Application.Models.AuthModels.Request;

public record SignInModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}