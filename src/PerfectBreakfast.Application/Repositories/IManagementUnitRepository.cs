using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Repositories;

public interface IManagementUnitRepository : IGenericRepository<ManagementUnit>
{
    public Task<List<ManagementUnit>> GetManagementUnits();
    public Task<List<ManagementUnit>> GetManagementUnitsByToday(DateTime dateTime);
    public Task<ManagementUnit?> GetManagementUintDetail(Guid id);
    Task<ManagementUnit?> GetManagementById(Guid id, params Expression<Func<ManagementUnit, object>>[] includeProperties);
}