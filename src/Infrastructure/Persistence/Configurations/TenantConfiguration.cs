using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable(DatabaseSchema.Tables.Tenant);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasMaxLength(200);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(50);

        builder.Property(t => t.Description).IsRequired().HasMaxLength(150);

        builder.HasMany(t => t.Locations)
            .WithMany("Tenants")
            .UsingEntity<Dictionary<string,object>>(
                "TenantLocation",
                j =>
                    j.HasOne<Location>()
                    .WithMany()
                    .HasForeignKey("LocationId"),
                j => j.HasOne<Tenant>()
                    .WithMany()
                    .HasForeignKey("TenantId")
            );

    }
}
