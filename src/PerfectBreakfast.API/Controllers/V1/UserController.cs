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
    private readonly IImgurService _imgurService;
    public UserController(IUserService userService, IImgurService imgurService)
    {
        _userService = userService;
        _imgurService = imgurService;
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id,UpdateUserRequestModel requestModel)
    {
        var response = await _userService.UpdateUser(id,requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
            var imageUrl = await _imgurService.UploadImageAsync(file);

            // Lưu imageUrl vào cơ sở dữ liệu
            // ...
            return Ok(imageUrl);
        
    }
}