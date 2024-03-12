using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface ISupplierFoodAssignmentRepository : IGenericRepository<SupplierFoodAssignment>
    {
        public Task<List<SupplierFoodAssignment>> GetByDailyOrder(Guid dailyOrderId);
        public Task<List<SupplierFoodAssignment>> GetByBookingDate();
    }
}
