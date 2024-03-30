using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IComboFoodRepository : IGenericRepository<ComboFood>
{
    public Task<List<Food?>> GetFoodByComboId(Guid comboId);
}