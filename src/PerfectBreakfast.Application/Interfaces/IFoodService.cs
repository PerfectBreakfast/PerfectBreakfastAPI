using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IFoodService
    {
        public Task<OperationResult<List<FoodResponse>>> GetAllFoods();
        public Task<OperationResult<FoodResponse>> GetFoodById(Guid foodId);
        public Task<OperationResult<FoodResponse>> CreateFood(CreateFoodRequestModels requestModel);
        public Task<OperationResult<FoodResponse>> UpdateFood(Guid foodId, UpdateFoodRequestModels requestModel);
        public Task<OperationResult<FoodResponse>> RemoveFood(Guid foodId);
    }
}
