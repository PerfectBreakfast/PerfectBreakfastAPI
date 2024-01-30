using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Repositories;

public interface IPartnerRepository : IGenericRepository<Partner>
{
    public Task<List<Partner>> GetManagementUnits();
    public Task<List<Partner>> GetManagementUnitsByToday(DateTime dateTime);
    public Task<Partner?> GetManagementUintDetail(Guid id);
    Task<Partner?> GetManagementById(Guid id, params Expression<Func<Partner, object>>[] includeProperties);
}