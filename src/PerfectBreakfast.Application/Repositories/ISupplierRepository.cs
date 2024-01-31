using System.Linq.Expressions;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    public Task<Supplier?> GetSupplierUintDetail(Guid id);
    public Task<List<Supplier>?> GetSupplierByPartner(Guid id);
    Task<Supplier?> GetSupplierById(Guid id,params Expression<Func<Supplier, object>>[] includeProperties);
}