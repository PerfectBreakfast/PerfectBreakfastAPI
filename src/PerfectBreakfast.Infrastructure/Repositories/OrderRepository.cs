using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }

        public Task<List<Order>> GetOrderByDailyOrderId(Guid dailyOrderId)
        {
            return _dbSet.Where(o => o.DailyOrderId == dailyOrderId && o.OrderStatus == OrderStatus.Paid)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                .ToListAsync();
        }


        public Task<Order> GetOrderByOrderCode(int orderCode)
        {
            return _dbSet.SingleAsync(x => x.OrderCode == orderCode);
        }
    }
}
