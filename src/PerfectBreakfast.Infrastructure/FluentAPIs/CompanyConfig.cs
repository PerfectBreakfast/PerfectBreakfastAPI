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
        builder.Property(x => x.PhoneNumber).HasMaxLength(10);
        
        builder.HasOne(x => x.Partner)
            .WithMany(x => x.Companies)
            .HasForeignKey(fk => fk.PartnerId);
        
        builder.HasOne(x => x.Delivery)
            .WithMany(x => x.Companies)
            .HasForeignKey(fk => fk.DeliveryId);
    }
}