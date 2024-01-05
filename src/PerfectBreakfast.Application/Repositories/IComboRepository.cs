using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IComboRepository : IGenericRepository<Combo>
    {
        Task<Combo> GetComboFoodByIdAsync(Guid id);
    }

}
