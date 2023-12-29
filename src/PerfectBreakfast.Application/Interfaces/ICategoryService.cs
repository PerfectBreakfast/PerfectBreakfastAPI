using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface ICategoryService
    {
        public Task<OperationResult<List<CategoryResponse>>> GetAllCategorys();
        public Task<OperationResult<CategoryResponse>> GetCategoryById(Guid categoryId);
        public Task<OperationResult<CategoryResponse>> CreateCategory(CreateCategoryRequest requestModel);
        public Task<OperationResult<CategoryResponse>> UpdateCategory(Guid categoryId, UpdateCategoryRequest requestModel);
        public Task<OperationResult<CategoryResponse>> RemoveCategory(Guid categoryId);
    }
}
