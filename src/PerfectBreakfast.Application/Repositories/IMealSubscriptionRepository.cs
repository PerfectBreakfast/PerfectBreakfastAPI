using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IMealSubscriptionRepository : IGenericRepository<MealSubscription>
{
    public Task<MealSubscription?> FindByCompanyId(Guid companyId, Guid mealId);
    public Task<List<MealSubscription>> GetByCompany(Guid companyId);
}