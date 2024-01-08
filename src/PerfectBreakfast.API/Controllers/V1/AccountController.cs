using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.AuthModels.Request;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("account")]
public class AccountController : BaseController
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signup")]
    [ApiVersionNeutral]
    public async Task<IActionResult> SignUp(SignUpModel request)
    {
        var response = await _userService.SignUp(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost("signin")]
    [ApiVersionNeutral]
    public async Task<IActionResult> SignIn(SignInModel request)
    {
        var response = await _userService.SignIn(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("refresh-user-token")]
    [Authorize]
    public async Task<IActionResult> RefreshUserToken()
    {
        var response = await _userService.RefreshUserToken();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}