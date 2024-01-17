﻿using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Request;

namespace PerfectBreakfast.API.Controllers.V1;
[Route("api/v{version:apiVersion}/supplyassigments")]
public class SupplyAssigmentController : BaseController
{
    private readonly ISupplyAssigmentService _supplyAssigmentService;

    public SupplyAssigmentController(ISupplyAssigmentService supplyAssigmentService)
    {
        _supplyAssigmentService = supplyAssigmentService;
    }
    [HttpGet]
    public async Task<IActionResult> GetSupplyAssigment()
    {
        var response = await _supplyAssigmentService.GetSupplyAssigment();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    [HttpPost]
    public async Task<IActionResult> CreateSupplyAssigment(CreateSupplyAssigment requestModel)
    {
        var response = await _supplyAssigmentService.CreateSupplyAssigment(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}