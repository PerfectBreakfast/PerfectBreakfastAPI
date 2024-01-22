using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class JWTService
{
    private readonly AppConfiguration _appConfiguration;
    private readonly UserManager<User> _userManager;

    public JWTService(AppConfiguration appConfiguration,UserManager<User> userManager)
    {
        _appConfiguration = appConfiguration;
        _userManager = userManager;
    }

    public async Task<UserLoginResponse> CreateJWT(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.JwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role,role)));
        var token = new JwtSecurityToken(
            claims: claims,
            issuer: _appConfiguration.JwtSettings.Issuer,
            audience: _appConfiguration.JwtSettings.Audience,
            expires: DateTime.Now.AddMinutes(_appConfiguration.JwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return new UserLoginResponse(roles.ToList(), new JwtSecurityTokenHandler().WriteToken(token));
    }
}