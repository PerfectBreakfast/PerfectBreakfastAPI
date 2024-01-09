using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class SupplierFoodAssignmentConfig : IEntityTypeConfiguration<SupplierFoodAssignment>
{
    public void Configure(EntityTypeBuilder<SupplierFoodAssignment> builder)
    {
        builder.ToTable("SupplierFoodAssignment");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ReceivedAmount).HasColumnType("decimal(18,2)");
        
        builder.HasOne(r => r.Food)
            .WithMany(ur => ur.SupplierFoodAssignments)
            .HasForeignKey(pk => pk.FoodId);
        
        builder.HasOne(r => r.SupplierCommissionRate)
            .WithMany(ur => ur.SupplierFoodAssignments)
            .HasForeignKey(pk => pk.SupplierCommissionRateId);
        
        builder.HasOne(r => r.DailyOrder)
            .WithMany(ur => ur.SupplierFoodAssignments)
            .HasForeignKey(pk => pk.DailyOrderId);
    }
}