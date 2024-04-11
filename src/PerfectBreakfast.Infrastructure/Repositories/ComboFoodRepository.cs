using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class ComboFoodRepository : GenericRepository<ComboFood>,IComboFoodRepository
{
    public ComboFoodRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
    {
    }


    public async Task<List<Food?>> GetFoodByComboId(Guid comboId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.ComboId == comboId)
            .Select(x => x.Food)
            .ToListAsync();
    }

    public async Task<bool> IsFoodExistInCombo(Guid foodId)
    {
        return _dbSet.AsNoTracking()
            .Any(x => x.FoodId == foodId);
    }
}