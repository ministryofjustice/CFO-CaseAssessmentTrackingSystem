using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Configuration;

public class ContractEntityTypeConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.Contract,
            DatabaseConstants.Schemas.Configuration
        );

        builder.Property(c => c.Id)
            .HasMaxLength(12);

        builder.Ignore(l => l.DomainEvents);

        builder.Property(c => c.LotNumber)
            .IsRequired();
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ContractDescription);

        builder.HasOne(c => c.Tenant)
            .WithMany()
            .HasForeignKey("_tenantId");

        builder.Property("_tenantId")
            .HasColumnName("TenantId")
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Navigation(c => c.Tenant)
            .AutoInclude();

        builder.HasIndex(c => c.LotNumber)
            .IsUnique();

        // Configure the Lifetime value object
        builder.OwnsOne(l => l.Lifetime, lifetime => {
            lifetime.Property(l => l.StartDate).HasColumnName("LifetimeStart");
            lifetime.Property(l => l.EndDate).HasColumnName("LifetimeEnd");
        });

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

    }
}
