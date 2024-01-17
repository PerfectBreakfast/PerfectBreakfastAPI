using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IManagementUnitRepository : IGenericRepository<ManagementUnit>
{
    public Task<ManagementUnit?> GetManagementUintDetail(Guid id);
}