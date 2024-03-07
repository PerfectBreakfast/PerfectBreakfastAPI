using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class DeliveryRepository : GenericRepository<Delivery>,IDeliveryRepository
{
    public DeliveryRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) 
        : base(context, timeService, claimsService)
    {
    }
    // to do
    public async Task<Delivery?> GetDeliveryById(Guid id, params Expression<Func<Delivery, object>>[] includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<List<Delivery>> GetDeliveriesByToday(DateTime dateTime)
    {
        var dateToCompare = dateTime.Date;
        return await _dbSet.Include(mu => mu.Companies)
            .ToListAsync();
    }
}