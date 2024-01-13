using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class ComboRepository : GenericRepository<Combo>, IComboRepository
    {
        public ComboRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }

        //to do
        public async Task<Combo> GetComboFoodByIdAsync(Guid? id)
        {
            return await _dbSet.Where(c => c.Id == id)
                            .Include(c => c.ComboFoods)
                            .ThenInclude(cf => cf.Food)
                            .FirstOrDefaultAsync();
        }
    }
}
