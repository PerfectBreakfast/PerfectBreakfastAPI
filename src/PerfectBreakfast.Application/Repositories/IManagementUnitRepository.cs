using System.Linq.Expressions;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IManagementUnitRepository : IGenericRepository<ManagementUnit>
{
    public Task<List<ManagementUnit>> GetManagementUnits(DateTime dateTime);
    public Task<ManagementUnit?> GetManagementUintDetail(Guid id);
    Task<ManagementUnit?> GetManagementById(Guid id,params Expression<Func<ManagementUnit, object>>[] includeProperties);
}