using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Utils;


namespace PerfectBreakfast.API.Controllers.V1;

/// <summary>
/// User Controller
/// </summary>
[Route("api/v{version:apiVersion}/users")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Api for All, Get all Users in system
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var response = await _userService.GetUsers();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for All login , Get user by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}"),Authorize]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var response = await _userService.GetUser(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for All 
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination"), Authorize(policy:  ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetUserPagination(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _userService.GetUserPaginationAsync(pageIndex,pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin, create user for deliveryUnit, managementUnit, supplier
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost,Authorize]
    public async Task<IActionResult> CreateUser([FromForm]CreateUserRequestModel requestModel)
    {
        var response = await _userService.CreateUser(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for All login
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id}"),Authorize]
    public async Task<IActionResult> UpdateUser(Guid id,[FromForm]UpdateUserRequestModel requestModel)
    {
        var response = await _userService.UpdateUser(id,requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for All login
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut ,Authorize]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileModel requestModel)
    {
        var response = await _userService.UpdateProfile(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Customer (hàm cập nhật thông tin sdt và công ty sau khi login google)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id}/google")]
    public async Task<IActionResult> UpdateUserLoginGoogle(Guid id,UpdateUserLoginGoogleRequest requestModel)
    {
        var response = await _userService.UpdateUserLoginGoogle(id,requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Update image user 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="image"></param>
    /// <returns></returns>
    [HttpPut("{id}/image"),Authorize]
    public async Task<IActionResult> UpdateImageUser(Guid id,IFormFile image)
    {
        var response = await _userService.UpdateImageUser(id,image);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Delivery Admin
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("deliverystaff/pagination"),Authorize(policy: ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> GetDeliveryStaffByDeliveryAdmin(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _userService.GetDeliveryStaffByDeliveryAdmin(pageIndex,pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    ///  Api for Delivery Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet("deliverystaff"),Authorize(policy: ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> GetDeliveryStaffByDeliveryAdminList()
    {
        var response = await _userService.GetDeliveryStaffByDeliveryAdminList();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API for all login
    /// </summary>
    /// <param name="changePassword"></param>
    /// <returns></returns>
    [HttpPut("change-password"), Authorize]
    public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
    {
        var response = await _userService.ChangePassword(changePassword);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}