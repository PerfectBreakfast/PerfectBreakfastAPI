using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
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
            return _dbSet.Where(o => o.DailyOrderId == dailyOrderId 
                                     && o.OrderStatus != OrderStatus.Pending 
                                     &&  o.OrderStatus != OrderStatus.Cancel 
                                     && o.OrderStatus != OrderStatus.Paid)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrderHistory(Guid userId,int pageNumber = 1, params IncludeInfo<Order>[] includeProperties)
        {
            var itemsQuery = _dbSet.Where(x => x.WorkerId == userId);
            itemsQuery = itemsQuery.OrderByDescending(x => x.CreationDate);
            itemsQuery = itemsQuery.Take(pageNumber);
            // Xử lý các thuộc tính include và thenInclude
            foreach (var includeProperty in includeProperties)
            {
                var queryWithInclude = itemsQuery.Include(includeProperty.NavigationProperty);
                foreach (var thenInclude in includeProperty.ThenIncludes)
                {
                    queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
                }
                itemsQuery = queryWithInclude;
            }
            return await itemsQuery.AsNoTracking().AsSplitQuery().ToListAsync();
        }

        public async Task<List<Order>> GetOrderHistoryByDeliveryStaff(Guid userId, int pageNumber, params IncludeInfo<Order>[] includeProperties)
        {
            var itemsQuery = _dbSet.Where(x => x.DeliveryStaffId == userId);
            itemsQuery = itemsQuery.OrderByDescending(x => x.CreationDate);
            itemsQuery = itemsQuery.Take(pageNumber);
            // Xử lý các thuộc tính include và thenInclude
            foreach (var includeProperty in includeProperties)
            {
                var queryWithInclude = itemsQuery.Include(includeProperty.NavigationProperty);
                foreach (var thenInclude in includeProperty.ThenIncludes)
                {
                    queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
                }
                itemsQuery = queryWithInclude;
            }
            return await itemsQuery.AsNoTracking().AsSplitQuery().ToListAsync();
        }

        public async Task<List<Order>> GetOrderByDate(DateOnly? fromDate, DateOnly? toDate)
        {
           
            return await _dbSet
                .Where(order => DateOnly.FromDateTime(order.CreationDate) >= fromDate 
                                && DateOnly.FromDateTime(order.CreationDate) <= toDate 
                                && (order.OrderStatus == OrderStatus.Complete || order.OrderStatus == OrderStatus.CancelledForOverdue))
                .Include(o => o.OrderDetails)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrderByDateForCompany(DateOnly? fromDate, DateOnly? toDate)
        {
            return await _dbSet
                .Where(order => DateOnly.FromDateTime(order.CreationDate) >= fromDate 
                                && DateOnly.FromDateTime(order.CreationDate) <= toDate 
                                && (order.OrderStatus == OrderStatus.Complete || order.OrderStatus == OrderStatus.CancelledForOverdue))
                .Include(o => o.DailyOrder)
                    .ThenInclude(d => d.MealSubscription)
                        .ThenInclude(m => m.Company)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrderByDateForComboAndFood(DateOnly? fromDate, DateOnly? toDate)
        {
            return await _dbSet
                .Where(order => DateOnly.FromDateTime(order.CreationDate) >= fromDate 
                                && DateOnly.FromDateTime(order.CreationDate) <= toDate 
                                && (order.OrderStatus == OrderStatus.Complete || order.OrderStatus == OrderStatus.CancelledForOverdue))
                .Include(o => o.OrderDetails)
                    .ThenInclude(o => o.Combo)
                        .ThenInclude(c => c.ComboFoods)
                            .ThenInclude(c => c.Food)
                .Include(o => o.OrderDetails)
                    .ThenInclude(o => o.Food)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AreAllOrdersCompleteForDailyOrder(Guid dailyOrderId)
        {
            // Kiểm tra xem có bất kỳ Order nào liên kết với DailyOrder cụ thể này đang còn ở đã thanh toán
            var incompleteOrders = await _dbSet
                .Where(o => o.DailyOrderId == dailyOrderId && o.OrderStatus == OrderStatus.Paid)
                .AsNoTracking()
                .ToListAsync();

            // Nếu không có Order nào (tức là tất cả đều 'complete' hoặc có những cái bị cancel thì không tính), trả về true
            return !incompleteOrders.Any();
        }

        public async Task<Order> GetOrderByOrderCode(long orderCode)
        {
            var order = await _dbSet.AsNoTracking().SingleAsync(x => x.OrderCode == orderCode);
            return order;
        }
    }
}
