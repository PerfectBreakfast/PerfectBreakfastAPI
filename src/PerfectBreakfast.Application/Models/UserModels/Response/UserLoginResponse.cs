namespace PerfectBreakfast.Application.Models.UserModels.Response;

public sealed record UserLoginResponse(Guid UserId, string Name,string? Email,string? Image,List<string?> Roles,string AccessToken,string RefreshToken);