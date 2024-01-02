using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class DeliveryAssignmentConfig : IEntityTypeConfiguration<DeliveryAssignment>
{
    public void Configure(EntityTypeBuilder<DeliveryAssignment> builder)
    {
        builder.ToTable("DeliveryAssignment");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.DeliveryUnit)
            .WithMany(x => x.DeliveryAssignments)
            .HasForeignKey(fk => fk.DeliveryUnitId);

        builder.HasOne(x => x.ManagementUnit)
            .WithMany(x => x.DeliveryAssignments)
            .HasForeignKey(fk => fk.ManagementUnitId);
    }
}