using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class SupplyAssignmentConfig : IEntityTypeConfiguration<SupplyAssignment>
{
    public void Configure(EntityTypeBuilder<SupplyAssignment> builder)
    {
        builder.ToTable("SupplyAssignment");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.SupplyAssignments)
            .HasForeignKey(fk => fk.SupplierId);

        builder.HasOne(x => x.ManagementUnit)
            .WithMany(x => x.SupplyAssignments)
            .HasForeignKey(fk => fk.ManagementUnitId);
    }
}