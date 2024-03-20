using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Interfaces;

public interface ICategoryService
{
    public Task<OperationResult<List<CategoryResponse>>> GetAllCategorys();
    public Task<OperationResult<CategoryDetailFood>> GetCategoryId(Guid categoryId, FoodStatus status);
    public Task<OperationResult<CategoryResponse>> CreateCategory(CreateCategoryRequest requestModel);
    public Task<OperationResult<CategoryResponse>> UpdateCategory(Guid categoryId, UpdateCategoryRequest requestModel);
    public Task<OperationResult<CategoryResponse>> RemoveCategory(Guid categoryId);
}