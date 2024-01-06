using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class CompanyConfig : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(300);
        
        builder.HasOne(x => x.ManagementUnit)
            .WithMany(x => x.Companies)
            .HasForeignKey(fk => fk.ManagementUnitId);
        
        builder.HasOne(x => x.DeliveryUnit)
            .WithMany(x => x.Companies)
            .HasForeignKey(fk => fk.DeliveryUnitId);
    }
}