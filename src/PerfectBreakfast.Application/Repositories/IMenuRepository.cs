using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        Task<Menu> GetMenuFoodByIdAsync(Guid id);
        Task<Menu?> GetMenuFoodByStatusAsync();
    }
}
