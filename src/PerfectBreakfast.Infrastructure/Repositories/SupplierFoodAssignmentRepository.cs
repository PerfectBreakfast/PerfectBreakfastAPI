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

        public async Task<List<SupplierFoodAssignment>> GetByDailyOrder(Guid dailyOrderId)
        {
            return await _dbSet.Where(s => s.DailyOrderId == dailyOrderId).ToListAsync();
        }

        public async Task<List<SupplierFoodAssignment>> GetByBookingDateForSupplier()
        {
            return await _dbSet.Where(s => s.Status == SupplierFoodAssignmentStatus.Confirmed)
                .Include(s => s.Partner)
                .Include(s => s.Food)
                .Include(s => s.SupplierCommissionRate)
                .Include(s => s.DailyOrder)
                    .ThenInclude(s => s.MealSubscription)
                .ToListAsync();
        }

        public async Task<List<SupplierFoodAssignment>> GetByForSuperAdmin()
        {
            return await _dbSet.Where(s => s.Status != SupplierFoodAssignmentStatus.Pending)
                .Include(s => s.Partner)
                .Include(s => s.Food)
                .Include(s => s.SupplierCommissionRate)
                .Include(s => s.DailyOrder)
                .ThenInclude(s => s.MealSubscription)
                .ToListAsync();
        }
    }

}
