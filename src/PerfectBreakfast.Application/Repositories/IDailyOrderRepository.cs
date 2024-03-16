using System.Linq.Expressions;
using PerfectBreakfast.Application.Commons;
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
        Task<Pagination<DailyOrder>> ToPaginationForPartnerAndDelivery(List<Guid> mealSubscriptionIds ,int pageNumber = 0, int pageSize = 10);
        Task<Pagination<DailyOrder>> ToPaginationForAllStatus(List<Guid> mealSubscriptionIds ,int pageNumber = 0, int pageSize = 10);
        Task<Pagination<DailyOrder>> ToPagination(List<Guid> mealSubscriptionIds ,int pageNumber = 0, int pageSize = 10);
    }
}
