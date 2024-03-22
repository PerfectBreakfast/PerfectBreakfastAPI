using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        private readonly AppDbContext _context;
        public MenuRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService)
            : base(context, timeService, claimsService)
        {
            _context = context;
        }

        //to do

        public async Task<Menu> GetMenuFoodByIdAsync(Guid id)
        {
            var u = await _dbSet.Where(c => c.Id == id)
                .Include(c => c.MenuFoods)
                    .ThenInclude(mf => mf.Combo)
                .Include(f => f.MenuFoods)
                    .ThenInclude(mf => mf.Food)
                .FirstOrDefaultAsync();
            return u;
        }

        public async Task<Menu?> GetMenuFoodByStatusAsync()
        {
            return await _dbSet.Where(c => c.IsSelected == true)
                .Include(c => c.MenuFoods)
                    .ThenInclude(mf => mf.Combo)
                        .ThenInclude(x => x.ComboFoods)
                            .ThenInclude(x => x.Food)
                .Include(f => f.MenuFoods)
                    .ThenInclude(mf => mf.Food)
                .SingleOrDefaultAsync();
            
        }
    }
}
