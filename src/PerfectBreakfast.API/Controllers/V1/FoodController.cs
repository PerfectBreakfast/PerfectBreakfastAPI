using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Services;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/foods")]
    public class FoodController : BaseController
    {
        private readonly IFoodService _foodService;
        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFood()
        {
            var response = await _foodService.GetAllFoods();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodId(Guid id)
        {
            var response = await _foodService.GetFoodById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood(CreateFoodRequestModels requestModel)
        {
            var response = await _foodService.CreateFood(requestModel);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFood(Guid id, UpdateFoodRequestModels requestModel)
        {
            var response = await _foodService.UpdateFood(id, requestModel);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFood(Guid id)
        {
            var response = await _foodService.RemoveFood(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetFoodPagination(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _foodService.GetFoodPaginationAsync(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
