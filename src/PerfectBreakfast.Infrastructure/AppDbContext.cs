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
    public DbSet<ActionHistory> ActionHistories { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Combo> Combos { get; set; }
    public DbSet<ComboFood> ComboFoods { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<DailyOrder> DailyOrders { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<MealSubscription> MealSubscriptions { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuFood> MenuFoods { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Partner> Partners { get; set; }
    public DbSet<PartnerPayment> PartnerPayments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<ShippingOrder> ShippingOrders { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<SupplierCommissionRate> SupplierCommissionRates { get; set; }
    public DbSet<SupplierFoodAssignment> SupplierFoodAssignments { get; set; }
    public DbSet<SupplyAssignment> SupplyAssignments { get; set; }
    
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
