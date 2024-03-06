using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Application.Utils;
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

    public async Task<UserLoginResponse> CreateJWT(User user, string refreshToken)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.JwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role,role)));
        var token = new JwtSecurityToken(
            claims: claims,
            issuer: _appConfiguration.JwtSettings.Issuer,
            audience: _appConfiguration.JwtSettings.Audience,
            expires: DateTime.UtcNow.AddMinutes(_appConfiguration.JwtSettings.ExpiryMinutes),
            //expires: DateTime.UtcNow.AddSeconds(15),
            signingCredentials: credentials);

        return new UserLoginResponse(user.Id,user.Name,user.Email,user.Image,roles.ToList(), new JwtSecurityTokenHandler().WriteToken(token),refreshToken);
    }
    
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_appConfiguration.JwtSettings.SecretKey)),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}