using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class MenuFoodRepository : GenericRepository<MenuFood>, IMenuFoodRepository
{
    public MenuFoodRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
    {
    }

    public async Task<bool> IsFoodExistInMenu(Guid foodId)
    {
        return _dbSet.AsNoTracking()
            .Any(x => x.FoodId == foodId);
    }

    public async Task<bool> IsComboExistInMenu(Guid comboId)
    {
        return _dbSet.AsNoTracking()
            .Any(x => x.ComboId == comboId);
    }
}