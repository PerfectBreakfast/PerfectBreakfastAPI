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

        public async Task<DailyOrder?> FindAllDataByCompanyId(Guid? mealSubscriptionId)
        {
            var a = await _dbSet.Where(d => d.MealSubscriptionId == mealSubscriptionId)
                .OrderByDescending(d => d.CreationDate)
                .Include(d => d.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(c => c.Combo)
                            .ThenInclude(m => m.MenuFoods)
                                .ThenInclude(f => f.Food)
                .FirstOrDefaultAsync();
            return a;
        }

        public async Task<DailyOrder?> FindByCompanyId(Guid? mealSubscriptionId)
        {
            return await _dbSet.Where(d => d.MealSubscriptionId == mealSubscriptionId && d.Status == DailyOrderStatus.Initial)
                .OrderByDescending(d => d.CreationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DailyOrder>> FindByBookingDate(DateTime dateTime)
        {
            return await _dbSet
                .Where(d => d.BookingDate == DateOnly.FromDateTime(dateTime).AddDays(1) &&
                            d.Status == DailyOrderStatus.Initial)
                .Include(d => d.Orders)
                .Include(d => d.MealSubscription)
                .ToListAsync();
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

        public async Task<DailyOrder> FindByMealSubscription(Guid? mealSubscriptionId)
        {
            var a = await _dbSet.Where(d => d.MealSubscriptionId == mealSubscriptionId)
                .OrderByDescending(d => d.CreationDate)
                .Include(d => d.Orders)
                .FirstOrDefaultAsync();
            return a;
        }

        public async Task<DailyOrder?> GetById(Guid id)
        {
            return await _dbSet.Where(d => d.Id == id)
                .Include(d => d.MealSubscription)
                .FirstOrDefaultAsync();
             
        }
    }
}
