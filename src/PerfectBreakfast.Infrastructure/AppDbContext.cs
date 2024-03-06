using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Domain.Entities;
using System.Reflection;

namespace PerfectBreakfast.Infrastructure;

public class AppDbContext : IdentityDbContext<User, Role, Guid,
    IdentityUserClaim<Guid>, UserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    // DbSet<> 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable(name: "User");
        });
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable(name: "Role");
        });
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable(name: "UserRoles");
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            
            entity.HasOne(ur => ur.Role)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });
        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable(name: "UserClaims");
        });
        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable(name: "UserLogins");
        });
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable(name: "RoleClaims");
        });
        modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable(name: "UserTokens");
        });
    }
}
