using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class DailyOrderRepository : GenericRepository<DailyOrder>, IDailyOrderRepository
    {
        public DailyOrderRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }

        public async Task<DailyOrder?> FindAllDataByCompanyId(Guid? companyId)
        {
            var a = await _dbSet.Where(d => d.CompanyId == companyId)
                .OrderByDescending(d => d.CreationDate)
                .Include(d => d.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(c => c.Combo)
                            .ThenInclude(m => m.MenuFoods)
                                .ThenInclude(f => f.Food)
                .FirstOrDefaultAsync();
            return a;
        }

        public async Task<bool> DailyOrderCreatedForDateAsync(DateTime date)
        {
            // Thực hiện truy vấn để kiểm tra xem đã có DailyOrder nào được tạo cho ngày đã cho hay không
            var existingDailyOrder = await _dbSet.AnyAsync(d => d.CreationDate.Date == date.Date);

            // Trả về kết quả kiểm tra
            return existingDailyOrder;
        }

        public async Task<DailyOrder?> FindByCompanyId(Guid? companyId)
        {
            return await _dbSet.Where(d => d.CompanyId == companyId)
                .OrderByDescending(d => d.CreationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DailyOrder>> FindByCreationDate(DateTime dateTime)
        {
            var dateToCompare = dateTime.Date;

            return await _dbSet
                .Where(d => d.CreationDate.Date == dateToCompare)
                .Include(d => d.Orders)
                .Include(d => d.Company)
                .ToListAsync();
        }
    }
}
