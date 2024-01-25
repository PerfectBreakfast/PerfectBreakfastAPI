using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class DeliveryUnitRepository : GenericRepository<DeliveryUnit>,IDeliveryUnitRepository
{
    public DeliveryUnitRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) 
        : base(context, timeService, claimsService)
    {
    }
    // to do
    public async Task<DeliveryUnit?> GetDeliveryUnitById(Guid id, params Expression<Func<DeliveryUnit, object>>[] includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
    }
}