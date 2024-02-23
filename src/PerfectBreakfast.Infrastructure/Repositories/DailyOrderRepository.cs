using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

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

        public async Task<bool> IsDailyOrderCreated(DateTime date)
        {
            // Thực hiện truy vấn để kiểm tra xem đã có DailyOrder nào được tạo cho ngày đã cho hay không
            var existingDailyOrder = await _dbSet
                .AnyAsync(d => 
                        d.Status == DailyOrderStatus.Initial && d.BookingDate == DateOnly.FromDateTime(date).AddDays(2) || d.CreationDate.AddDays(1) == date);

            // Trả về kết quả kiểm tra
            return existingDailyOrder;
        }

        public async Task<DailyOrder?> FindByCompanyId(Guid? companyId)
        {
            return await _dbSet.Where(d => d.CompanyId == companyId && d.Status == DailyOrderStatus.Initial)
                .OrderByDescending(d => d.CreationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DailyOrder>> FindByCreationDate(DateTime dateTime)
        {
            return await _dbSet
                .Where(d => d.BookingDate == DateOnly.FromDateTime(dateTime).AddDays(1) && d.Status == DailyOrderStatus.Initial)
                .Include(d => d.Orders)
                .Include(d => d.Company)
                .ToListAsync();
        }
    }
}
