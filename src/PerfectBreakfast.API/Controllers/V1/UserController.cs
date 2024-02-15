using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.UserModels.Request;


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
    /// Api for All , Get user by Id
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
    [HttpGet("pagination")]
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
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromForm]CreateUserRequestModel requestModel)
    {
        var response = await _userService.CreateUser(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for All
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id,UpdateUserRequestModel requestModel)
    {
        var response = await _userService.UpdateUser(id,requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Update image user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="image"></param>
    /// <returns></returns>
    [HttpPut("{id}/image")]
    public async Task<IActionResult> UpdateImageUser(Guid id,IFormFile image)
    {
        var response = await _userService.UpdateImageUser(id,image);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    [HttpGet("deliverystaff")]
    public async Task<IActionResult> GetDeliveryStaffByDelieryAdmin(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _userService.GetDeliveryStaffByDelieryAdmin(pageIndex,pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}