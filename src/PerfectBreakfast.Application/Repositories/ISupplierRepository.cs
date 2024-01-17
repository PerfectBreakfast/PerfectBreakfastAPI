using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    public Task<Supplier?> GetSupplierUintDetail(Guid id);
}