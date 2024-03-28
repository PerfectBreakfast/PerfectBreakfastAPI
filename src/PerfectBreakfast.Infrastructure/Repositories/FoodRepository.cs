using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class FoodRepository : GenericRepository<Food>, IFoodRepository
    {
        public FoodRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }

        public async Task<List<Food>> GetFoodForSupplier(Guid id)
        {
            return await _dbSet
                .Where(f => !f.SupplierCommissionRates.Any(scr => scr.SupplierId == id) && !f.IsDeleted)
                .ToListAsync();
        }
    }
}
