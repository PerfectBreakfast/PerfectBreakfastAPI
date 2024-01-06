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
        
        builder.HasOne(x => x.DeliveryUnit)
            .WithMany(x => x.PartnerPayments)
            .HasForeignKey(fk => fk.DeliveryUnitId);
        
        builder.HasOne(x => x.ManagementUnit)
            .WithMany(x => x.PartnerPayments)
            .HasForeignKey(fk => fk.ManagementUnitId);
        
        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.PartnerPayments)
            .HasForeignKey(fk => fk.SupplierId);
    }
}