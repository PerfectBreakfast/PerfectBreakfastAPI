using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class SupplyAssignmentConfig : IEntityTypeConfiguration<SupplyAssignment>
{
    public void Configure(EntityTypeBuilder<SupplyAssignment> builder)
    {
        builder.ToTable("SupplyAssignment");
        builder.HasKey(x => new {x.PartnerId,x.SupplierId});
        
        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.SupplyAssignments)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(fk => fk.SupplierId);

        builder.HasOne(x => x.Partner)
            .WithMany(x => x.SupplyAssignments)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(fk => fk.PartnerId);
    }
}