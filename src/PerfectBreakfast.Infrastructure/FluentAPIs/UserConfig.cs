using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Email).IsRequired().HasMaxLength(50);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(50);

        builder.HasOne(c => c.Company)
            .WithMany(u => u.Workers)
            .HasForeignKey(pk => pk.CompanyId);
        
        builder.HasOne(c => c.DeliveryUnit)
            .WithMany(u => u.Users)
            .HasForeignKey(pk => pk.DeliveryUnitId);
        
        builder.HasOne(c => c.ManagementUnit)
            .WithMany(u => u.Users)
            .HasForeignKey(pk => pk.ManagementUnitId);
        
        builder.HasOne(c => c.Supplier)
            .WithMany(u => u.Users)
            .HasForeignKey(pk => pk.SupplierId);
        
        builder.HasOne(c => c.Role)
            .WithMany(u => u.Users)
            .HasForeignKey(pk => pk.RoleId);
    }
}
