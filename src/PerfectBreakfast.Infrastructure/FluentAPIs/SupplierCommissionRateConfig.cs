using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class SupplierCommissionRateConfig : IEntityTypeConfiguration<SupplierCommissionRate>
{
    public void Configure(EntityTypeBuilder<SupplierCommissionRate> builder)
    {
        builder.ToTable("SupplierCommissionRate");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.SupplierCommissionRates)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(fk => fk.SupplierId);

        builder.HasOne(x => x.Food)
            .WithMany(x => x.SupplierCommissionRates)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(fk => fk.FoodId);
    }
}