using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IMenuFoodRepository : IGenericRepository<MenuFood>
{
    public Task<bool> IsFoodExistInMenu(Guid foodId);
    public Task<bool> IsComboExistInMenu(Guid comboId);
}