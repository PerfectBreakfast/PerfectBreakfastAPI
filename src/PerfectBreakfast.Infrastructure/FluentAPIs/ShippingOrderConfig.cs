using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class ShippingOrderConfig : IEntityTypeConfiguration<ShippingOrder>
{
    public void Configure(EntityTypeBuilder<ShippingOrder> builder)
    {
        builder.ToTable("ShippingOrder");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.DailyOrder)
            .WithMany(x => x.ShippingOrders)
            .HasForeignKey(fk => fk.DailyOrderId);
        
        builder.HasOne(x => x.Shipper)
            .WithMany(x => x.ShippingOrders)
            .HasForeignKey(fk => fk.ShipperId);
        
        // Create Index 
        builder.HasIndex(x => x.IsDeleted);
    }
}