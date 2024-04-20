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

    public Task<bool> IsFoodExistInMenu(Guid foodId)
    {
        return Task.FromResult(_dbSet.AsNoTracking()
            .Any(x => x.FoodId == foodId));
    }

    public Task<bool> IsComboExistInMenu(Guid comboId)
    {
        return Task.FromResult(_dbSet.AsNoTracking()
            .Any(x => x.ComboId == comboId));
    }
}