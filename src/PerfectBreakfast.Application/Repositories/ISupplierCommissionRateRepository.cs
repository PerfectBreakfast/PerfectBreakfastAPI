using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Repositories;

public interface ISupplierCommissionRateRepository : IGenericRepository<SupplierCommissionRate>
{

    Task<bool> AnyAsync(Expression<Func<SupplierCommissionRate, bool>> predicate);
    Task<List<SupplierCommissionRate>?> GetBySupplierId(Guid id);
}