using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/users")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await _userService.GetAllUsers();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var response = await _userService.GetUserById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("pagination")]
    public async Task<IActionResult> GetUserPagination(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _userService.GetUserPaginationAsync(pageIndex,pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}