using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;


namespace PerfectBreakfast.Infrastructure.Repositories;

public class ShippingOrderRepository : GenericRepository<ShippingOrder>, IShippingOrderRepository
{
    public ShippingOrderRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
    {
    }
}
