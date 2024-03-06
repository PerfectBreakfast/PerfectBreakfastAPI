using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class MealSubscriptionConfig : IEntityTypeConfiguration<MealSubscription>
{
    public void Configure(EntityTypeBuilder<MealSubscription> builder)
    {
        builder.ToTable("MealSubscription");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(r => r.Company)
            .WithMany(ur => ur.MealSubscriptions)
            .HasForeignKey(pk => pk.CompanyId);
        
        builder.HasOne(r => r.Meal)
            .WithMany(ur => ur.MealSubscriptions)
            .HasForeignKey(pk => pk.MealId);
    }
}