using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class DailyOrderConfig : IEntityTypeConfiguration<DailyOrder>
{
    public void Configure(EntityTypeBuilder<DailyOrder> builder)
    {
        builder.ToTable("DailyOrder");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
        builder.HasIndex(x => x.BookingDate);
        
        
        builder.HasOne(r => r.MealSubscription)
            .WithMany(ur => ur.DailyOrders)
            .HasForeignKey(pk => pk.MealSubscriptionId);
        
        // Thêm ràng buộc duy nhất cho CompanyId và BookingDate
        //builder.HasIndex(x => new { x.CompanyId, x.BookingDate }).IsUnique();
    }
}