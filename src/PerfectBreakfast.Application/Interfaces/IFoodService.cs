using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IFoodService
    {
        public Task<OperationResult<List<FoodResponse>>> GetAllFoods();
        public Task<OperationResult<List<FoodResponse>>> GetFoodByFoodStatus(FoodStatus status);
        public Task<OperationResult<FoodResponseCategory>> GetFoodById(Guid foodId);
        public Task<OperationResult<FoodResponse>> CreateFood(CreateFoodRequestModels requestModel);
        public Task<OperationResult<FoodResponse>> UpdateFood(Guid foodId, UpdateFoodRequestModels requestModel);
        public Task<OperationResult<FoodResponse>> RemoveFood(Guid foodId);
        public Task<OperationResult<Pagination<FoodResponse>>> GetFoodPaginationAsync(string? searchTerm, int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<TotalFoodForPartnerResponse>> GetFoodsForPartner(Guid dailyOrderId);
        public Task<OperationResult<TotalFoodForPartnerResponse>> GetFoodsForDelivery(Guid dailyOrderId);
        public Task<OperationResult<List<FoodResponse>>> GetFoodForSupplier(Guid id);
    }
}
