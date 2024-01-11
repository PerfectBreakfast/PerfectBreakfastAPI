using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.OrderCode).ValueGeneratedOnAdd().UseMySqlIdentityColumn();
        
        builder.HasOne(r => r.Worker)
            .WithMany(ur => ur.OrdersWorker)
            .HasForeignKey(pk => pk.WorkerId);
        
        builder.HasOne(r => r.DailyOrder)
            .WithMany(ur => ur.Orders)
            .HasForeignKey(pk => pk.DailyOrderId);
        
        builder.HasOne(r => r.ManagementUnit)
            .WithMany(ur => ur.Orders)
            .HasForeignKey(pk => pk.ManagementUnitId);

        builder.HasOne(x => x.PaymentMethod)
            .WithOne(x => x.Order)
            .HasForeignKey<PaymentMethod>(fk => fk.OrderId);
    }
}

/*public Guid? WorkerId { get; set; }
public Guid? ShipperId { get; set; }
public Guid? DeliveryUnitId { get; set; }
public Guid? ManagementUnitId { get; set; }
public Guid? SupplierId { get; set; }*/