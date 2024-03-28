using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class ComboConfig : IEntityTypeConfiguration<Combo>
{
    public void Configure(EntityTypeBuilder<Combo> builder)
    {
        builder.ToTable("Combo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        
        // create index 
        builder.HasIndex(x => x.IsDeleted);
    }
}