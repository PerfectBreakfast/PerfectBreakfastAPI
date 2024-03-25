using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/foods")]
public class FoodController : BaseController
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet,Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetAllFood()
    {
        var response = await _foodService.GetAllFoods();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Super Admin ( 0-combo   1-Retail )
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [HttpGet("status"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetFoodByFoodStatus(FoodStatus status)
    {
        var response = await _foodService.GetFoodByFoodStatus(status);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin , Customer 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}"), Authorize(Roles = $"{ConstantRole.CUSTOMER},{ConstantRole.SUPER_ADMIN}")]
    public async Task<IActionResult> GetFoodId(Guid id)
    {
        var response = await _foodService.GetFoodById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin - FoodStatus: 0 là combo, 1 là bán lẻ
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreateFood([FromForm] CreateFoodRequestModels requestModel)
    {
        var response = await _foodService.CreateFood(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin - FoodStatus: 0 là combo, 1 là bán lẻ
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> UpdateFood(Guid id, [FromForm] UpdateFoodRequestModels requestModel)
    {
        var response = await _foodService.UpdateFood(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> RemoveFood(Guid id)
    {
        var response = await _foodService.RemoveFood(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetFoodPagination(string? searchTerm, int pageIndex = 0, int pageSize = 10)
    {
        var response = await _foodService.GetFoodPaginationAsync(searchTerm, pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Partner Admin-API lấy danh sách món theo daily order
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id}/dailyorderid/partner"), Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetFoodForPartner(Guid id)
    {
        var response = await _foodService.GetFoodsForPartner(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Delivery Admin, Delivery Staff-API lấy danh sách món theo daily order
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id}/dailyorderid/delivery")]
    [Authorize(Roles = $"{ConstantRole.DELIVERY_ADMIN},{ConstantRole.DELIVERY_STAFF}")]
    public async Task<IActionResult> GetFoodForDelivery(Guid id)
    {
        var response = await _foodService.GetFoodsForDelivery(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Super Admin - API lấy các món ăn cho NCC đăng kí
    /// </summary>
    /// <returns></returns>
    [HttpGet ("supplier-id"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetFoodForSupplier(Guid id)
    {
        var response = await _foodService.GetFoodForSupplier(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}