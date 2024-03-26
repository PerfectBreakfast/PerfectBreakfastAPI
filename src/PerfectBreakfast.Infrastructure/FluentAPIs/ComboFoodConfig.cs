using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class ComboFoodConfig : IEntityTypeConfiguration<ComboFood>
{
    public void Configure(EntityTypeBuilder<ComboFood> builder)
    {
        builder.ToTable("ComboFood");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.Combo)
            .WithMany(x => x.ComboFoods)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(fk => fk.ComboId);
        
        builder.HasOne(x => x.Food)
            .WithMany(x => x.ComboFoods)
            .HasForeignKey(fk => fk.FoodId);
        
        // Create Index 
        builder.HasIndex(x => x.IsDeleted);
    }
}