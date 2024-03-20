using System.Security.Claims;
using PerfectBreakfast.Application.Models.UserModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IJwtService
{
    public Task<UserLoginResponse> CreateJWT(string email, string refreshToken);
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}