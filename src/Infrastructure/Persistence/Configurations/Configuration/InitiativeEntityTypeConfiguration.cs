using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Configuration;

public class InitiativeEntityTypeConfiguration : IEntityTypeConfiguration<Initiative>
{
    public void Configure(EntityTypeBuilder<Initiative> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.Initiative,
            DatabaseConstants.Schemas.Configuration
        );

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Code)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.InitiativeCode);

        builder.HasIndex(f => f.Code)
            .IsUnique();

        builder.Property(f => f.Description)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.InitiativeDescription);

        builder.OwnsOne(f => f.Lifetime, lifetime =>
        {
            lifetime.Property(l => l.StartDate).HasColumnName("LifetimeStart").IsRequired();
            lifetime.Property(l => l.EndDate).HasColumnName("LifetimeEnd").IsRequired();
        });

        builder.HasOne(f => f.Contract)
            .WithMany()
            .HasForeignKey("_contractId")
            .IsRequired();

        builder.Property<string>("_contractId")
            .HasColumnName("ContractId")
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ContractId);

        builder.Ignore(f => f.DomainEvents);

        builder.Property(f => f.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(f => f.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
