using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService)
            : base(context, timeService, claimsService)
        {
        }

        //to do

        public async Task<Menu> GetMenuFoodByIdAsync(Guid id)
        {
            return await _dbSet.Where(c => c.Id == id)
                            .Include(c => c.MenuFoods)
                            .ThenInclude(mf => mf.Food)
                            .ThenInclude(f => f.MenuFoods)
                            .ThenInclude(mf => mf.Combo)
                            .FirstOrDefaultAsync();
        }

    }
}
