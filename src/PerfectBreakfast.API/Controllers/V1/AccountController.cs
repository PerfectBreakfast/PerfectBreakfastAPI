using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.AuthModels.Request;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/account")]
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
    
    [HttpPost("deliverystaff/login")]
    [ApiVersionNeutral]
    public async Task<IActionResult> SignInDeliveryStaff(SignInModel request)
    {
        var response = await _userService.DeliveryStaffSignIn(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost("refresh-user-token")]
    public async Task<IActionResult> RefreshUserToken(TokenModel tokenModel)
    {
        var response = await _userService.RefreshUserToken(tokenModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("current-user")]
    [ApiVersionNeutral]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var response = await _userService.GetCurrentUser();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}