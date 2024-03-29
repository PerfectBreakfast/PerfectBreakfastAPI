﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/categories")]
public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// API FOR SUPPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetAllCategory()
    {
        var response = await _categoryService.GetAllCategorys();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin (0-combo  1-retail)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    [HttpGet("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetCategoryById(Guid id,FoodStatus status)
    {
        var response = await _categoryService.GetCategoryId(id,status);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API FOR SUPPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequest requestModel)
    {
        var response = await _categoryService.CreateCategory(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API FOR SUPPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryRequest requestModel)
    {
        var response = await _categoryService.UpdateCategory(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API FOR SUPPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> RemoveCategory(Guid id)
    {
        var response = await _categoryService.RemoveCategory(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}