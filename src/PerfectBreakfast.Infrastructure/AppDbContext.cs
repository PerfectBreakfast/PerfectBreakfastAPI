using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Domain.Entities;
using System.Reflection;

namespace PerfectBreakfast.Infrastructure;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    //public DbSet<User> Users { get; set; }
    //public DbSet<Role> Roles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<DeliveryUnit> DeliveryUnits { get; set; }
    public DbSet<ManagementUnit> ManagementUnits { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderHistory> OrderHistories { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<DeliveryAssignment> DeliveryAssignments { get; set; }
    public DbSet<SupplyAssignment> SupplyAssignments { get; set; }
    public DbSet<MenuFood> MenuFoods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable(name: "User");
        });
        modelBuilder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable(name: "Role");
        });
        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable(name: "UserRoles");
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
