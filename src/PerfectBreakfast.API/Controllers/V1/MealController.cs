using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MealModels.Request;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/meals")]
public class MealController : BaseController
{
    private readonly IMealService _mealService;

    public MealController(IMealService mealService)
    {
        _mealService = mealService;
    }
    
    /// <summary>
    /// Api for All
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetMeals()
    {
        var response = await _mealService.GetMeals();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateMeal(CreateMealRequest request)
    {
        var response = await _mealService.CreateMeal(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}