using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.IdentityModel.Tokens;
using PerfectBreakfast.API.Contracts.Commons;
using PerfectBreakfast.Application.Commons;

namespace PerfectBreakfast.API.Middlewares;

public class AuthorizationMiddlewareHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler DefaultHandler = new AuthorizationMiddlewareResultHandler();
    private readonly AppConfiguration _appConfiguration;

    public AuthorizationMiddlewareHandler(AppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
    }
    
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // lấy ra token từ trong request
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if(authorizeResult.Challenged)
        {
            var validationResult = ValidateToken(token);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            //response body
            await context.Response.WriteAsJsonAsync(new ErrorResponse(
                 401, validationResult, validationResult, DateTime.UtcNow.AddHours(7)
            ));
            return;
        }
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            //response body
            await context.Response.WriteAsJsonAsync(new ErrorResponse(
                403, "Forbidden", "Permission: You do not have permission to access this resource", DateTime.UtcNow.AddHours(7)
            ));
            return;
        }
        await DefaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
    
    private string ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        try
        {
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        }
        catch (SecurityTokenExpiredException)
        {
            return "Token Expired";
        }
        catch (Exception)
        {
            return "Token Invalid";
        }
        return "Token Invalid";
    }
    
    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.JwtSettings.SecretKey)),
            ValidateIssuer = false,
            //ValidIssuer = _issuer,
            ValidateAudience = false,
            //ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Ensure tokens are expired exactly at token expiration time
        };
    }
}