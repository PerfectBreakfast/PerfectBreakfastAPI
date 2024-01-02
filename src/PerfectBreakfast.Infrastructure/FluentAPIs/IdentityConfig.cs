using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class IdentityConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
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

    }
}

