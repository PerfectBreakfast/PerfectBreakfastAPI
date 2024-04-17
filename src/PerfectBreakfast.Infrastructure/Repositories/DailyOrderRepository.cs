using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class DailyOrderRepository : GenericRepository<DailyOrder>, IDailyOrderRepository
    {
        public DailyOrderRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }
        

        public async Task<List<DailyOrder>> FindByBookingDate(DateTime dateTime)
        {
            return await _dbSet
                .Where(d => d.BookingDate == DateOnly.FromDateTime(dateTime).AddDays(1) &&
                            d.Status == DailyOrderStatus.Initial)
                .Include(d => d.Orders)
                .Include(d => d.MealSubscription)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<string?>> FindPartnerAndDeliveryAdminEmailsByBookingDateAndStatus(DateTime dateTime)
        {
            // Lưu ý: Ta không thực hiện async/await ở đây vì chưa thực sự truy vấn database
            var partnerEmailsQuery = _dbSet
                .Where(d => d.BookingDate == DateOnly.FromDateTime(dateTime).AddDays(1) &&
                            d.Status == DailyOrderStatus.Processing)
                .SelectMany(d => d.MealSubscription!.Company!.Partner!.Users)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == ConstantRole.PARTNER_ADMIN))
                .Select(u => u.Email);

            var deliveryEmailsQuery = _dbSet
                .Where(d => d.BookingDate == DateOnly.FromDateTime(dateTime).AddDays(1) &&
                            d.Status == DailyOrderStatus.Processing)
                .SelectMany(d => d.MealSubscription!.Company!.Delivery!.Users)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == ConstantRole.DELIVERY_ADMIN))
                .Select(u => u.Email);

            // Kết hợp hai truy vấn và thực hiện một lần async/await với ToListAsync
            var combinedEmails = partnerEmailsQuery.Concat(deliveryEmailsQuery)
                .Distinct() // Loại bỏ email trùng lặp
                .AsNoTracking();

            // await chỉ áp dụng cho ToListAsync() - thực hiện truy vấn và trả về kết quả
            return await combinedEmails.ToListAsync();
        }

        public async Task<bool> IsDailyOrderCreated(DateTime date)
        {
            // Thực hiện truy vấn để kiểm tra xem đã có DailyOrder nào được tạo cho ngày đã cho hay không
            var existingDailyOrder = await _dbSet
                .AsNoTracking()
                .AnyAsync(d => 
                    d.Status == DailyOrderStatus.Initial && d.BookingDate == DateOnly.FromDateTime(date).AddDays(2) || d.CreationDate.AddDays(1) == date);

            // Trả về kết quả kiểm tra
            return existingDailyOrder;
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

        public async Task<Pagination<DailyOrder>> ToPagination(List<Guid> mealSubscriptionIds,DailyOrderStatus? status = null, int pageNumber = 0, int pageSize = 10)
        {
            // Bắt đầu bằng việc tạo một IQueryable cho phép bạn xây dựng truy vấn một cách linh hoạt
            var query = _dbSet.AsQueryable();

            // Lọc dựa trên mealSubscriptionIds và điều kiện khác
            // nếu k có status thì lấy bth
            if (status is null)
            {
                query = query
                    .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) && d.MealSubscription != null &&
                                d.MealSubscription.Company != null && d.OrderQuantity > 0);
            }
            else
            {
                query = query
                    .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) && d.MealSubscription != null &&
                                d.MealSubscription.Company != null && d.OrderQuantity > 0 && d.Status == status);
            }

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

        public async Task<List<DailyOrder>> GetByMeal(List<Guid> mealSubscriptionIds, DateOnly? fromDate, DateOnly? toDate)
        {
            return await _dbSet
                .Where(d => mealSubscriptionIds.Contains(d.MealSubscriptionId.Value) 
                            && d.MealSubscription != null 
                            && d.MealSubscription.Company != null
                            && d.BookingDate <= toDate 
                            && d.BookingDate >= fromDate)
                .Include(d => d.MealSubscription)
                    .ThenInclude(ms => ms.Company)
                        .ThenInclude(ms => ms.Partner)
                .Include(d => d.MealSubscription)
                    .ThenInclude(ms => ms.Company)
                        .ThenInclude(ms => ms.Delivery)
                .Include(d => d.MealSubscription)
                    .ThenInclude(ms => ms.Meal)
                .Include(d => d.Orders)
                .OrderByDescending(d => d.BookingDate)
                .ToListAsync();
        }

        public async Task<List<DailyOrder>> GetForStatistic(DateOnly? fromDate, DateOnly? toDate)
        {
            return await _dbSet
                .Where(d => d.OrderQuantity > 0 && d.BookingDate <= toDate && d.BookingDate >= fromDate)
                .Include(d => d.MealSubscription)
                    .ThenInclude(ms => ms.Company)
                        .ThenInclude(ms => ms.Partner)
                .Include(d => d.MealSubscription)
                    .ThenInclude(ms => ms.Company)
                        .ThenInclude(ms => ms.Delivery)
                .Include(d => d.MealSubscription)
                    .ThenInclude(ms => ms.Meal)
                .Include(d => d.Orders)
                .OrderByDescending(d => d.BookingDate)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<DailyOrder?> GetDailyOrderByPackageId(Guid packageId)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.PackageId == packageId);
        }

    }
}
