using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories
{
    public class SupplierFoodAssignmentRepository : GenericRepository<SupplierFoodAssignment>, ISupplierFoodAssignmentRepository
    {
        public SupplierFoodAssignmentRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }
    }

}
