using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class PartnerPaymentConfig : IEntityTypeConfiguration<PartnerPayment>
{
    public void Configure(EntityTypeBuilder<PartnerPayment> builder)
    {
        builder.ToTable("PartnerPayment");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
        
        builder.HasOne(x => x.SupperAdmin)
            .WithMany(x => x.PartnerPayments)
            .HasForeignKey(fk => fk.SupperAdminId);
        
        builder.HasOne(x => x.DailyOrder)
            .WithMany(x => x.PartnerPayments)
            .HasForeignKey(fk => fk.DailyOrderId);
        
        builder.HasOne(x => x.Delivery)
            .WithMany(x => x.PartnerPayments)
            .HasForeignKey(fk => fk.DeliveryId);
        
        builder.HasOne(x => x.Partner)
            .WithMany(x => x.PartnerPayments)
            .HasForeignKey(fk => fk.PartnerId);
        
        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.PartnerPayments)
            .HasForeignKey(fk => fk.SupplierId);
    }
}