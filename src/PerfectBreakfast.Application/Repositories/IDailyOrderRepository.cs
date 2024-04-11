using System.Linq.Expressions;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IDailyOrderRepository : IGenericRepository<DailyOrder>
    {
        public Task<List<DailyOrder>> FindByBookingDate(DateTime dateTime);
        public Task<List<string?>> FindPartnerAndDeliveryAdminEmailsByBookingDateAndStatus(DateTime dateTime);
        public Task<bool> IsDailyOrderCreated(DateTime date);
        Task<Pagination<DailyOrder>> ToPaginationProcessingForPartner(List<Guid> mealSubscriptionIds ,int pageNumber = 0, int pageSize = 10);
        Task<Pagination<DailyOrder>> ToPaginationForDeliveryDistribution(List<Guid> mealSubscriptionIds ,int pageNumber = 0, int pageSize = 10);
        Task<Pagination<DailyOrder>> ToPaginationForDelivery(List<Guid> mealSubscriptionIds ,int pageNumber = 0, int pageSize = 10);
        Task<Pagination<DailyOrder>> ToPaginationForPartner(List<Guid> mealSubscriptionIds ,int pageNumber = 0, int pageSize = 10);

        Task<Pagination<DailyOrder>> ToPagination(List<Guid> mealSubscriptionIds ,DailyOrderStatus? status = null,int pageNumber = 0, int pageSize = 10);
        public Task<List<DailyOrder>> GetByMeal(List<Guid> mealSubscriptionIds, DateOnly? fromDate, DateOnly? toDate);
        public Task<List<DailyOrder>> GetForStatistic(DateOnly? fromDate, DateOnly? toDate);

    }
}
