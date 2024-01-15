using System.Linq.Expressions;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface ISupplierCommissionRateRepository : IGenericRepository<SupplierCommissionRate>
{
    
    Task<bool> AnyAsync(Expression<Func<SupplierCommissionRate, bool>> predicate);
}