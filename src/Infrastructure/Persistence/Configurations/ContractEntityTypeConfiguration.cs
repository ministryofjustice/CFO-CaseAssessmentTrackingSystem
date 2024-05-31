using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Contract=Cfo.Cats.Domain.Entities.Administration.Contract;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class ContractEntityTypeConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable(
        DatabaseSchema.Tables.Contract
        );

        builder.Property(c => c.Id)
            .HasMaxLength(12);

        builder.Ignore(l => l.DomainEvents);

        builder.Property(c => c.LotNumber)
            .IsRequired();
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey("_tenantId");

        builder.Property("_tenantId")
            .HasColumnName("TenantId");

        builder.Navigation(e => e.Tenant)
            .AutoInclude();

        builder.HasIndex(c => c.LotNumber)
            .IsUnique();

        // Configure the Lifetime value object
        builder.OwnsOne(l => l.Lifetime, lifetime => {
            lifetime.Property(l => l.StartDate).HasColumnName("LifetimeStart");
            lifetime.Property(l => l.EndDate).HasColumnName("LifetimeEnd");
        });

    }
}
