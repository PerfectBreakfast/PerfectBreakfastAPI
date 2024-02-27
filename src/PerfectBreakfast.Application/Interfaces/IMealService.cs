using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.MealModels.Request;
using PerfectBreakfast.Application.Models.MealModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IMealService
{
    public Task<OperationResult<List<MealResponse>>> GetMeals();
    public Task<OperationResult<MealResponse>> CreateMeal(CreateMealRequest request);
}