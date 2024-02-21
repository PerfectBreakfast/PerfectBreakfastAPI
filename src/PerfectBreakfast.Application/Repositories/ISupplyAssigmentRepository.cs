using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface ISupplyAssigmentRepository : IBaseRepository<SupplyAssignment>
{
    public Task<bool> IsDuplicateAssignment(Guid partnerId, Guid supplierId);
}