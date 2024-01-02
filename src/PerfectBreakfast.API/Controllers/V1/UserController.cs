using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> GetUsers()
    {
        var response = await _userService.GetUsers();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{id}"),Authorize]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var response = await _userService.GetUser(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("pagination")]
    public async Task<IActionResult> GetUserPagination(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _userService.GetUserPaginationAsync(pageIndex,pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequestModel requestModel)
    {
        var response = await _userService.CreateUser(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}