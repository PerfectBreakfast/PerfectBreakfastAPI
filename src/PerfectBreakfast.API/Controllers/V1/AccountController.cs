using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.AuthModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Request;

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
    public async Task<IActionResult> SignInForCustomer(SignInModel request)
    {
        var response = await _userService.SignIn(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost("google")]
    [ApiVersionNeutral]
    public async Task<IActionResult> GoogleLogin(string code)
    {
        var response = await _userService.ExternalLogin(code);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// (Api login cho tất cả role quản lý)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("management/login")]
    [ApiVersionNeutral]
    public async Task<IActionResult> ManagementLogin(ManagementLoginModel request)
    {
        var response = await _userService.ManagementLogin(request);
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
    
    /// <summary>
    /// API for all
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet("password-token")]
    public async Task<IActionResult> GeneratePasswordResetToken(string email)
    {
        var response = await _userService.GeneratePasswordResetToken(email);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API for all
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("password-resetion")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var response = await _userService.ResetPassword(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}