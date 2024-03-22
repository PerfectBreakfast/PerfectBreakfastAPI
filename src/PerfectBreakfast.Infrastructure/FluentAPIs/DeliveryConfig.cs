using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class DeliveryConfig : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("Delivery");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Address).IsRequired().HasMaxLength(200);
        builder.Property(x => x.PhoneNumber).HasMaxLength(10);
        builder.Property(x => x.CommissionRate).HasColumnType("decimal(18,2)");
        builder.HasIndex(x => x.Name).IsUnique();
    }
}