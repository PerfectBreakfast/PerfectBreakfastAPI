using Microsoft.AspNetCore.Identity;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public UnitCode UnitCode { get; set; } 
    public ICollection<UserRole?>? UserRoles { get; set; }
}