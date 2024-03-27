using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
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
            return await _dbSet.AsNoTracking().Where(d => d.Id == id)
                .Include(d => d.MealSubscription)
                .FirstOrDefaultAsync();
             
        }

        public async Task<Pagination<DailyOrder>> ToPaginationProcessingForPartner(List<Guid> mealSubscriptionIds, int pageNumber = 0, int pageSize = 10)
        {
            // Bắt đầu bằng việc tạo một IQueryable cho phép bạn xây dựng truy vấn một cách linh hoạt
            var query = _dbSet.AsQueryable();

            // Lọc dựa trên mealSubscriptionIds và điều kiện khác
            query = query
                .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) && d.MealSubscription != null && d.MealSubscription.Company != null)
                .Where(d => d.OrderQuantity > 0 && d.Status == DailyOrderStatus.Processing);

            // Sử dụng AsNoTracking() để tối ưu hiệu suất vì dữ liệu không cần được theo dõi cho mục đích cập nhật
            query = query.AsNoTracking();

            // Đếm tổng số phần tử thỏa mãn điều kiện trước khi phân trang
            var totalItemsCount = await query.CountAsync();

            // Bổ sung các thao tác Include sau khi đã lọc để tránh tải dữ liệu không cần thiết
            query = query
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Company)
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Meal);

            // Thêm sắp xếp, phân trang và chia tách truy vấn
            var items = await query
                .OrderByDescending(d => d.BookingDate)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            // Tạo và trả về kết quả phân trang
            return new Pagination<DailyOrder>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalItemsCount = totalItemsCount,
                Items = items,
            };
        }

        public async Task<Pagination<DailyOrder>> ToPaginationForDeliveryDistribution(List<Guid> mealSubscriptionIds, int pageNumber = 0, int pageSize = 10)
        {
            // Bắt đầu bằng việc tạo một IQueryable cho phép bạn xây dựng truy vấn một cách linh hoạt
            var query = _dbSet.AsQueryable();

            // Lọc dựa trên mealSubscriptionIds và điều kiện khác
            query = query
                .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) && d.MealSubscription != null && d.MealSubscription.Company != null)
                .Where(d => d.OrderQuantity > 0 && d.Status != DailyOrderStatus.Initial && d.Status != DailyOrderStatus.Delivering && d.Status != DailyOrderStatus.Complete);

            // Sử dụng AsNoTracking() để tối ưu hiệu suất vì dữ liệu không cần được theo dõi cho mục đích cập nhật
            query = query.AsNoTracking();

            // Đếm tổng số phần tử thỏa mãn điều kiện trước khi phân trang
            var totalItemsCount = await query.CountAsync();

            // Bổ sung các thao tác Include sau khi đã lọc để tránh tải dữ liệu không cần thiết
            query = query
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Company)
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Meal);

            // Thêm sắp xếp, phân trang và chia tách truy vấn
            var items = await query
                .OrderByDescending(d => d.BookingDate)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            // Tạo và trả về kết quả phân trang
            return new Pagination<DailyOrder>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalItemsCount = totalItemsCount,
                Items = items,
            };
        }

        public async Task<Pagination<DailyOrder>> ToPaginationForDelivery(List<Guid> mealSubscriptionIds, int pageNumber = 0, int pageSize = 10)
        {
            // Bắt đầu bằng việc tạo một IQueryable cho phép bạn xây dựng truy vấn một cách linh hoạt
            var query = _dbSet.AsQueryable();

            // Lọc dựa trên mealSubscriptionIds và điều kiện khác
            query = query
                .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) && d.MealSubscription != null && d.MealSubscription.Company != null)
                .Where(d => d.OrderQuantity > 0 && d.Status == DailyOrderStatus.Complete || d.Status == DailyOrderStatus.Delivering && d.OrderQuantity > 0);

            // Sử dụng AsNoTracking() để tối ưu hiệu suất vì dữ liệu không cần được theo dõi cho mục đích cập nhật
            query = query.AsNoTracking();

            // Đếm tổng số phần tử thỏa mãn điều kiện trước khi phân trang
            var totalItemsCount = await query.CountAsync();

            // Bổ sung các thao tác Include sau khi đã lọc để tránh tải dữ liệu không cần thiết
            query = query
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Company)
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Meal);

            // Thêm sắp xếp, phân trang và chia tách truy vấn
            var items = await query
                .OrderByDescending(d => d.BookingDate)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            // Tạo và trả về kết quả phân trang
            return new Pagination<DailyOrder>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalItemsCount = totalItemsCount,
                Items = items,
            };
        }

        public async Task<Pagination<DailyOrder>> ToPaginationForPartner(List<Guid> mealSubscriptionIds, int pageNumber = 0, int pageSize = 10)
        {
            // Bắt đầu bằng việc tạo một IQueryable cho phép bạn xây dựng truy vấn một cách linh hoạt
            var query = _dbSet.AsQueryable();

            // Lọc dựa trên mealSubscriptionIds và điều kiện khác
            query = query
                .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) && d.MealSubscription != null && d.MealSubscription.Company != null)
                .Where(d => d.OrderQuantity > 0);

            // Sử dụng AsNoTracking() để tối ưu hiệu suất vì dữ liệu không cần được theo dõi cho mục đích cập nhật
            query = query.AsNoTracking();

            // Đếm tổng số phần tử thỏa mãn điều kiện trước khi phân trang
            var totalItemsCount = await query.CountAsync();

            // Bổ sung các thao tác Include sau khi đã lọc để tránh tải dữ liệu không cần thiết
            query = query
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Company)
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Meal);

            // Thêm sắp xếp, phân trang và chia tách truy vấn
            var items = await query
                .OrderByDescending(d => d.BookingDate)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            // Tạo và trả về kết quả phân trang
            return new Pagination<DailyOrder>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalItemsCount = totalItemsCount,
                Items = items,
            };
        }

        public async Task<Pagination<DailyOrder>> ToPagination(List<Guid> mealSubscriptionIds, int pageNumber = 0, int pageSize = 10)
        {
            // Bắt đầu bằng việc tạo một IQueryable cho phép bạn xây dựng truy vấn một cách linh hoạt
            var query = _dbSet.AsQueryable();

            // Lọc dựa trên mealSubscriptionIds và điều kiện khác
            query = query
                .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) && d.MealSubscription != null && d.MealSubscription.Company != null)
                .Where(d => d.OrderQuantity > 0);

            // Sử dụng AsNoTracking() để tối ưu hiệu suất vì dữ liệu không cần được theo dõi cho mục đích cập nhật
            query = query.AsNoTracking();

            // Đếm tổng số phần tử thỏa mãn điều kiện trước khi phân trang
            var totalItemsCount = await query.CountAsync();

            // Bổ sung các thao tác Include sau khi đã lọc để tránh tải dữ liệu không cần thiết
            query = query
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Company)
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Meal);

            // Thêm sắp xếp, phân trang và chia tách truy vấn
            var items = await query
                .OrderByDescending(d => d.BookingDate)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            // Tạo và trả về kết quả phân trang
            return new Pagination<DailyOrder>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalItemsCount = totalItemsCount,
                Items = items,
            };
        }

        public async Task<List<DailyOrder>> GetByMeal(List<Guid> mealSubscriptionIds)
        {
            return await _dbSet
                .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) && d.MealSubscription != null && d.MealSubscription.Company != null)
                .Include(d => d.MealSubscription)
                    .ThenInclude(ms => ms.Company)
                .Include(d => d.MealSubscription)
                    .ThenInclude(ms => ms.Meal)
                .Include(d => d.Orders)
                .OrderByDescending(d => d.BookingDate)
                .ToListAsync();
        }

        public async Task<List<DailyOrder>> GetForStatistic()
        {
            return await _dbSet
                .Where(d => d.OrderQuantity > 0)
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Company)
                .Include(d => d.MealSubscription)
                .ThenInclude(ms => ms.Meal)
                .OrderByDescending(d => d.BookingDate)
                .AsSplitQuery()
                .ToListAsync();
        }
    }
}
