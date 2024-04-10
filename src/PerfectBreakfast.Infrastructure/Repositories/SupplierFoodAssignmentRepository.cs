using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class SupplierFoodAssignmentRepository : GenericRepository<SupplierFoodAssignment>, ISupplierFoodAssignmentRepository
    {
        private ISupplierFoodAssignmentRepository _supplierFoodAssignmentRepositoryImplementation;

        public SupplierFoodAssignmentRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }

        public async Task<List<SupplierFoodAssignment>> GetForSupplierByPackage(List<Guid> packageIds, Guid supplierId)
        {
            var supplierAssignments = await _dbSet
                .Where(s => s.SupplierCommissionRate.SupplierId == supplierId && s.Status != SupplierFoodAssignmentStatus.Declined)
                .Include(s => s.Food)
                .Include(s => s.SupplierCommissionRate)
                .Include(s => s.Package)
                    .ThenInclude(s => s.Partner)
                .ToListAsync();
            
            var filteredAssignments = supplierAssignments
                .Where(s => packageIds.Contains(s.PackageId!.Value))
                .ToList();

            return filteredAssignments;
        }

        public async Task<List<SupplierFoodAssignment>> GetForPartnerByPackage(List<Guid> packageIds, Guid partnerId, Guid supplierId)
        {
            var supplierAssignments = await _dbSet
                .Where(s => s.Package.PartnerId == partnerId && s.SupplierCommissionRate.SupplierId == supplierId)
                .Include(s => s.Food)
                .Include(s => s.SupplierCommissionRate)
                .Include(s => s.Package)
                    .ThenInclude(s => s.DailyOrder)
                        .ThenInclude(s => s.MealSubscription)
                            .ThenInclude(s => s.Company)
                .ToListAsync();
            
            var filteredAssignments = supplierAssignments
                .Where(s => packageIds.Contains(s.PackageId!.Value))
                .ToList();

            return filteredAssignments;
        }

        public Task<List<SupplierFoodAssignment>> GetByPackage(Guid packageId)
        {
            return _dbSet.Where(s => s.PackageId == packageId).ToListAsync();
        }

        public async Task<Pagination<SupplierFoodAssignment>> ToPaginationByPartner(List<Guid> packageId, int pageNumber = 0, int pageSize = 10)
        {
            var representativeIds = await _dbSet
                .Where(d => packageId.Contains(d.PackageId.Value))
                .GroupBy(x => new { x.PackageId.Value, x.SupplierCommissionRate.SupplierId })
                .Select(g => g.FirstOrDefault().Id) // Lấy ID của mục đầu tiên trong mỗi nhóm
                .ToListAsync();
            // Bắt đầu bằng việc tạo một IQueryable cho phép bạn xây dựng truy vấn một cách linh hoạt
            var query = _dbSet.AsQueryable();
            
            // Sử dụng AsNoTracking() để tối ưu hiệu suất vì dữ liệu không cần được theo dõi cho mục đích cập nhật
            //query = query.AsNoTracking();

            // Đếm tổng số phần tử thỏa mãn điều kiện trước khi phân trang
            var totalItemsCount = representativeIds.Count;

            query = query.Where(d => representativeIds.Contains(d.Id))
                .Include(d => d.SupplierCommissionRate)
                    .ThenInclude(s => s.Supplier)
                .Include(d => d.Package)
                    .ThenInclude(s => s.DailyOrder)
                        .ThenInclude(s => s.MealSubscription)
                            .ThenInclude(s => s.Meal)
                .Include(d => d.Package)
                    .ThenInclude(s => s.SupplierFoodAssignments)
                        .ThenInclude(s => s.SupplierCommissionRate);
            
            // Thêm sắp xếp, phân trang và chia tách truy vấn
            var items = await query
                .OrderByDescending(d => d.Package.DailyOrder.BookingDate)
                    .ThenBy(d => d.DeliveryDeadline)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();
            
            // Tạo và trả về kết quả phân trang
            return new Pagination<SupplierFoodAssignment>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalItemsCount = totalItemsCount,
                Items = items,
            };
        }

        public async Task<Pagination<SupplierFoodAssignment>> ToPaginationBySupplier(List<Guid> commissionRateIds, int pageNumber = 0, int pageSize = 10)
        {
            var representativeIds = await _dbSet
                .Where(d => commissionRateIds.Contains(d.SupplierCommissionRateId.Value) && d.Status != SupplierFoodAssignmentStatus.Declined)
                .GroupBy(d => d.PackageId.Value)
                .Select(g => g.FirstOrDefault().Id)
                .ToListAsync();
            
            // Bắt đầu bằng việc tạo một IQueryable cho phép bạn xây dựng truy vấn một cách linh hoạt
            var query = _dbSet.AsQueryable();
            
            // Sử dụng AsNoTracking() để tối ưu hiệu suất vì dữ liệu không cần được theo dõi cho mục đích cập nhật
            //query = query.AsNoTracking();

            // Đếm tổng số phần tử thỏa mãn điều kiện trước khi phân trang
            var totalItemsCount = representativeIds.Count;

            query = query.Where(d => representativeIds.Contains(d.Id))
                .Include(d => d.Package)
                    .ThenInclude(s => s.Partner)
                .Include(d => d.Package)
                    .ThenInclude(s => s.DailyOrder)
                        .ThenInclude(s => s.MealSubscription)
                            .ThenInclude(s => s.Meal)
                .Include(d => d.Package)
                    .ThenInclude(s => s.SupplierFoodAssignments)
                        .ThenInclude(s => s.SupplierCommissionRate);
            
            // Thêm sắp xếp, phân trang và chia tách truy vấn
            var items = await query
                .OrderByDescending(d => d.Package.DailyOrder.BookingDate)
                    .ThenBy(d => d.DeliveryDeadline)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();
            
            // Tạo và trả về kết quả phân trang
            return new Pagination<SupplierFoodAssignment>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalItemsCount = totalItemsCount,
                Items = items,
            };
        }

        public async Task<List<SupplierFoodAssignment>> GetByBookingDateForSupplier()
        {
            // return await _dbSet.Where(s => s.Status == SupplierFoodAssignmentStatus.Confirmed)
            //     .Include(s => s.Partner)
            //     .Include(s => s.Food)
            //     .Include(s => s.SupplierCommissionRate)
            //     .Include(s => s.DailyOrder)
            //         .ThenInclude(s => s.MealSubscription)
            //     .ToListAsync();
            throw new NotImplementedException();
        }

        public async Task<List<SupplierFoodAssignment>> GetByForSuperAdmin()
        {
            // return await _dbSet.Where(s => s.Status != SupplierFoodAssignmentStatus.Pending)
            //     .Include(s => s.Partner)
            //     .Include(s => s.Food)
            //     .Include(s => s.SupplierCommissionRate)
            //     .Include(s => s.DailyOrder)
            //     .ThenInclude(s => s.MealSubscription)
            //     .ToListAsync();
            throw new NotImplementedException();
        }
    }

}
