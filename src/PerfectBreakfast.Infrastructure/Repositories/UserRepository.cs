using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context,ICurrentTime currentTime, IClaimsService claimsService) 
        : base(context,currentTime,claimsService)
    {
        
    }
    // to do
    
}