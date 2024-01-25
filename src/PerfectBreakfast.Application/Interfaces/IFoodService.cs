using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IFoodService
    {
        public Task<OperationResult<List<FoodResponse>>> GetAllFoods();
        public Task<OperationResult<FoodResponeCategory>> GetFoodById(Guid foodId);
        public Task<OperationResult<FoodResponse>> CreateFood(CreateFoodRequestModels requestModel);
        public Task<OperationResult<FoodResponse>> UpdateFood(Guid foodId, UpdateFoodRequestModels requestModel);
        public Task<OperationResult<FoodResponse>> RemoveFood(Guid foodId);
        public Task<OperationResult<Pagination<FoodResponse>>> GetFoodPaginationAsync(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<List<TotalFoodResponse>>> GetFoodsForManagementUnit();
    }
}
