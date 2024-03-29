using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.FluentAPIs;

public class PackageConfig : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("Package");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(r => r.DailyOrder)
            .WithOne(ur => ur.Package)
            .HasForeignKey<Package>(pk => pk.DailyOrderId)
            .IsRequired(false);
        
        builder.HasOne(x => x.Partner)
            .WithMany(x => x.Packages)
            .HasForeignKey(fk => fk.PartnerId);
    }
}