using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class RoleRepository : BaseRepository<Role>,IRoleRepository
{
    
    public RoleRepository(AppDbContext context) : base(context)
    {
    }
    
}
