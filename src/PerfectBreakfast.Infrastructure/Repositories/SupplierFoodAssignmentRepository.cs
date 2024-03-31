using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class SupplierFoodAssignmentRepository : GenericRepository<SupplierFoodAssignment>, ISupplierFoodAssignmentRepository
    {
        public SupplierFoodAssignmentRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }

        public async Task<List<SupplierFoodAssignment>> GetForSupplierByPackage(List<Guid?> packageIds, Guid supplierId)
        {
            var supplierAssignments = await _dbSet
                .Where(s => s.SupplierCommissionRate.SupplierId == supplierId)
                .Include(s => s.Food)
                .Include(s => s.SupplierCommissionRate)
                .Include(s => s.Package)
                    .ThenInclude(s => s.DailyOrder)
                        .ThenInclude(s => s.MealSubscription)
                            .ThenInclude(s => s.Company)
                .ToListAsync();
            
            var filteredAssignments = supplierAssignments
                .Where(s => packageIds.Contains(s.PackageId))
                .ToList();

            return filteredAssignments;
        }

        public async Task<List<SupplierFoodAssignment>> GetForPartnerByPackage(List<Guid?> packageIds, Guid partnerId)
        {
            var supplierAssignments = await _dbSet
                .Where(s => s.Package.PartnerId == partnerId)
                .Include(s => s.Food)
                .Include(s => s.SupplierCommissionRate)
                .Include(s => s.Package)
                    .ThenInclude(s => s.DailyOrder)
                        .ThenInclude(s => s.MealSubscription)
                            .ThenInclude(s => s.Company)
                .ToListAsync();
            
            var filteredAssignments = supplierAssignments
                .Where(s => packageIds.Contains(s.PackageId))
                .ToList();

            return filteredAssignments;
        }

        public Task<List<SupplierFoodAssignment>> GetByPackage(Guid packageId)
        {
            return _dbSet.Where(s => s.PackageId == packageId).ToListAsync();
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
