using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class MenuFoodConfig : IEntityTypeConfiguration<MenuFood>
{
    public void Configure(EntityTypeBuilder<MenuFood> builder)
    {
        builder.ToTable("MenuFood");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.Menu)
            .WithMany(x => x.MenuFoods)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(fk => fk.MenuId);
        
        builder.HasOne(x => x.Food)
            .WithMany(x => x.MenuFoods)
            .HasForeignKey(fk => fk.FoodId);
        
        builder.HasOne(x => x.Combo)
            .WithMany(x => x.MenuFoods)
            .HasForeignKey(fk => fk.ComboId);
        
        // Create Index 
        builder.HasIndex(x => x.IsDeleted);
    }
}