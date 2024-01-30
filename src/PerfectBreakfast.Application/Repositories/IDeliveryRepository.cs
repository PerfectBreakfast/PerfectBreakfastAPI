using System.Linq.Expressions;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IDeliveryRepository : IGenericRepository<Delivery>
{
    Task<Delivery?> GetDeliveryUnitById(Guid id,params Expression<Func<Delivery, object>>[] includeProperties);
}