using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IManagementUnitRepository : IGenericRepository<ManagementUnit>
{
    public Task<List<ManagementUnit>> GetManagementUnits(DateTime dateTime);
    public Task<ManagementUnit?> GetManagementUintDetail(Guid id);
}