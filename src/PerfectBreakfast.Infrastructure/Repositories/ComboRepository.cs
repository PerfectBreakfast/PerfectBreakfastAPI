using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class ComboRepository : GenericRepository<Combo>, IComboRepository
{
    public ComboRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context,
        timeService, claimsService)
    {
    }

    public async Task<List<Combo>> GetAllComboNotDeleted()
    {
        return await _dbSet
            .Where(x => !x.IsDeleted)
            .Include(c => c.ComboFoods)
            .ThenInclude(cf => cf.Food)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();
    }


    //to do
    public async Task<Combo?> GetComboFoodByIdAsync(Guid? id)
    {
        return await _dbSet.Where(c => c.Id == id)
            .Include(c => c.ComboFoods)
            .ThenInclude(cf => cf.Food)
            .ThenInclude(f => f.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public Task<Pagination<ComboResponse>> GetAllCombosWithPaginationAsync(int pageIndex, int pageSize,
        List<ComboResponse> combos)
    {
        // Tạo một IQueryable từ danh sách combo
        var comboQuery = combos.AsQueryable();

        // Thực hiện phân trang
        var itemCount = comboQuery.Count();
        var items = comboQuery
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToList(); // Sử dụng ToList thay vì ToListAsync

        // Tạo đối tượng Pagination<ComboResponse>
        var result = new Pagination<ComboResponse>()
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItemsCount = itemCount,
            Items = items,
        };

        return Task.FromResult(result);
    }
}