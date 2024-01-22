namespace PerfectBreakfast.Application.Models.UserModels.Response;

public sealed record UserLoginResponse(List<string> Roles,string AccessToken);