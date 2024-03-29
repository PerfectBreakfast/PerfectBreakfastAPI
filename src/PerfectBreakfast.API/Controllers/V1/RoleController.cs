﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.RoleModels.Request;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/roles")]
public class RoleController : BaseController
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        this._roleService = roleService;
    }

    /// <summary>
    /// Api get tất cả các role quản lý và nhân viên 
    /// </summary>
    /// <returns></returns>
    [HttpGet("management-role")]
    public async Task<IActionResult> GetManagementRole()
    {
        var response = await _roleService.GetManagementRole();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleId(Guid id)
    {
        var response = await _roleService.GetRoleById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for all Login (Get tất cả các role của 1 đơn vị)
    /// </summary>
    /// <param name="unitId"></param>
    /// <returns></returns>
    [HttpGet("unit/{unitId}"),Authorize]
    public async Task<IActionResult> GetRoleByUnitId(Guid unitId)
    {
        var response = await _roleService.GetRoleByUnitId(unitId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateRole(CreatRoleRequest requestModel)
    {
        var response = await _roleService.CreateRole(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, UpdateRolerequest requestModel)
    {
        var response = await _roleService.UpdateRole(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveRole(Guid id)
    {
        var response = await _roleService.RemoveRole(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}