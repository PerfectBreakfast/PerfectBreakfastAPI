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
        
        builder.HasOne(c => c.Delivery)
            .WithMany(u => u.Users)
            .HasForeignKey(pk => pk.DeliveryId);
        
        builder.HasOne(c => c.Partner)
            .WithMany(u => u.Users)
            .HasForeignKey(pk => pk.PartnerId);
        
        builder.HasOne(c => c.Supplier)
            .WithMany(u => u.Users)
            .HasForeignKey(pk => pk.SupplierId);

    }
}

