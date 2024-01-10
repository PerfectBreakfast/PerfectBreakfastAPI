using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;
using Claim = System.Security.Claims.Claim;

namespace PerfectBreakfast.Application.Utils;

public static class GenerateTokenString
{
    public static string? GenerateJsonWebToken(this User user, JwtSettings jwtSettings)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        };
        //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role,role)));
        var token = new JwtSecurityToken(
            claims: claims,
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            expires: DateTime.Now.AddMinutes(jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}