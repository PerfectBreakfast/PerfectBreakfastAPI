using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class OrderDetailConfig : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetail");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderDetails)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(fk => fk.OrderId);
        
        builder.HasOne(x => x.Food)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(fk => fk.FoodId);
        
        builder.HasOne(x => x.Combo)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(fk => fk.ComboId);
    }
}