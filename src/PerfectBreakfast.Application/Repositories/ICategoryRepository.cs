using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    public Task<Category> GetCategoryDetail(Guid id, FoodStatus status);
}