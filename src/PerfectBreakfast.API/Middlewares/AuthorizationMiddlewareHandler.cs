using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using PerfectBreakfast.API.Contracts.Commons;

namespace PerfectBreakfast.API.Middlewares;

public class AuthorizationMiddlewareHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler DefaultHandler = new AuthorizationMiddlewareResultHandler();
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if(authorizeResult.Challenged)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //response body
            await context.Response.WriteAsJsonAsync(new ErrorResponse(
                 401, "UnAuthorize", "UnAuthorized: Access is Denied due invalid credential", DateTime.UtcNow.AddHours(7)
            ));
            return;
        }
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //response body
            await context.Response.WriteAsJsonAsync(new ErrorResponse(
                403, "Forbidden", "Permission: You do not have permission to access this resource", DateTime.UtcNow.AddHours(7)
            ));
            return;
        }
        await DefaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}