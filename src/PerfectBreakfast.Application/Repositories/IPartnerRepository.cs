using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Repositories;

public interface IPartnerRepository : IGenericRepository<Partner>
{
    public Task<List<Partner>> GetPartners();
    public Task<List<Partner>> GetPartnersByToday(DateTime dateTime);
    public Task<Partner?> GetPartnerDetail(Guid id);
    Task<Partner?> GetPartnerById(Guid id, params Expression<Func<Partner, object>>[] includeProperties);
}