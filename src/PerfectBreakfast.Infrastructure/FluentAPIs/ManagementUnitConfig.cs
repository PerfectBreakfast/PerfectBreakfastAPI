using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class ManagementUnitConfig : IEntityTypeConfiguration<ManagementUnit>
{
    public void Configure(EntityTypeBuilder<ManagementUnit> builder)
    {
        builder.ToTable("ManagementUnit");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Address).IsRequired().HasMaxLength(200);
    }
}