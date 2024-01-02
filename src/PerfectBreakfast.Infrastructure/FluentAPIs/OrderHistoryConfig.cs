using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class OrderHistoryConfig : IEntityTypeConfiguration<OrderHistory>
{
    public void Configure(EntityTypeBuilder<OrderHistory> builder)
    {
        builder.ToTable("OrderHistory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.OrderHistories)
            .HasForeignKey(fk => fk.UserId);
        
        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderHistories)
            .HasForeignKey(fk => fk.OrderId);
    }
}