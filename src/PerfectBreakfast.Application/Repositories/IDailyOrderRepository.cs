using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IDailyOrderRepository : IGenericRepository<DailyOrder>
    {
        public Task<DailyOrder?> FindByCompanyId(Guid? companyId);
        public Task<DailyOrder?> FindAllDataByCompanyId(Guid? mealSubscriptionId);
        public Task<List<DailyOrder>> FindByBookingDate(DateTime dateTime);
        public Task<bool> IsDailyOrderCreated(DateTime date);
        public Task<DailyOrder> FindByMealSubscription(Guid? mealSubscriptionId);
        public Task<DailyOrder?> GetById(Guid id);
    }
}
