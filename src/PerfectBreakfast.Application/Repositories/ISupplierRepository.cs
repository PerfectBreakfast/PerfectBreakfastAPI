using System.Linq.Expressions;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    public Task<Supplier?> GetSupplierDetail(Guid id);
    public Task<List<Supplier>?> GetSupplierByPartner(Guid id);
    public Task<Supplier?> GetSupplierById(Guid id,params Expression<Func<Supplier, object>>[] includeProperties);
    public Task<List<Supplier>?> GetSupplierFoodAssignmentByPartner(Guid id);
    public Task<Supplier?> GetSupplierFoodAssignmentBySupplier(Guid id);
}