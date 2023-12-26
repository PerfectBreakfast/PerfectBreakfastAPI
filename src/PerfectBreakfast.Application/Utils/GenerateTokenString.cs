using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Utils;

public static class GenerateTokenString
{
    public static string? GenerateJsonWebToken(this User user, JwtSettings jwtSettings, DateTime now)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("PhoneNumber", user.PhoneNumber),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            //new Claim(ClaimTypes.Role,user.Role!.RoleName),
            new Claim("RoleId",user.Role.Id.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            expires: now.AddMinutes(jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}