using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Configuration;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Tenant, 
            DatabaseConstants.Schemas.Configuration);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(DatabaseConstants.FieldLengths.TenantName);

        builder.Property(t => t.Description).IsRequired().HasMaxLength(150);

        builder.HasMany(t => t.Locations)
            .WithMany(l => l.Tenants)
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

        builder.OwnsMany(l => l.Domains, domain => {
            domain.WithOwner().HasForeignKey("TenantId");
            domain.HasKey("TenantId", "Domain");
            domain.ToTable(DatabaseConstants.Tables.TenantDomain);
            domain.Property(x => x.Domain).HasMaxLength(255);
            domain.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            domain.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        });

        builder.Navigation(x => x.Domains)
            .AutoInclude();
        
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(36);
        
        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(36);
        
    }
}
