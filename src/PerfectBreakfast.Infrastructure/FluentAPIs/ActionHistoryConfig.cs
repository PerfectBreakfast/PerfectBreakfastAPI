using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class ActionHistoryConfig : IEntityTypeConfiguration<ActionHistory>
{
    public void Configure(EntityTypeBuilder<ActionHistory> builder)
    {
        builder.ToTable("ActionHistory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.ActionHistories)
            .HasForeignKey(fk => fk.UserId);
        
        builder.HasOne(x => x.DailyOrder)
            .WithMany(x => x.ActionHistories)
            .HasForeignKey(fk => fk.DailyOrderId);
    }
}