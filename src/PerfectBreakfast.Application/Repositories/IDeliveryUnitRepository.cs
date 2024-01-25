using System.Linq.Expressions;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IDeliveryUnitRepository : IGenericRepository<DeliveryUnit>
{
    Task<DeliveryUnit?> GetDeliveryUnitById(Guid id,params Expression<Func<DeliveryUnit, object>>[] includeProperties);
}