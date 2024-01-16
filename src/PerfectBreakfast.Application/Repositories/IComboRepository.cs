using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IComboRepository : IGenericRepository<Combo>
    {
        Task<Combo> GetComboFoodByIdAsync(Guid? id);
        Task<Pagination<ComboResponse>> GetAllCombosWithPaginationAsync(int pageIndex, int pageSize, List<ComboResponse> combos = null);
        Task<List<Combo>> GetAllCombo();
    }

}
