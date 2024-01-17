using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IDailyOrderRepository : IGenericRepository<DailyOrder>
    {
        public Task<DailyOrder> FindByCompanyId(Guid? companyId);
        public Task<List<DailyOrder>> FindByCreationDate(DateTime dateTime);
        public Task<DailyOrder> FindAllDataByCompanyId(Guid? companyId);
    }
}
