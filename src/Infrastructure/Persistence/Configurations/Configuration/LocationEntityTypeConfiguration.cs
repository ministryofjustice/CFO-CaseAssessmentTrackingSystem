using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Configuration;

public class LocationEntityTypeConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {

        builder.ToTable(
            DatabaseConstants.Tables.Location,
            DatabaseConstants.Schemas.Configuration
        );

        // Configure the primary key
        builder.HasKey(l => l.Id);

        // Configure properties
        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(200);// Adjust the length as needed

        builder.Property<int>("_genderProvisionId")
            .HasColumnName("GenderProvisionId")
            .IsRequired();

        builder.Property<int>("_locationTypeId")
            .HasColumnName("LocationTypeId")
            .IsRequired();

        // Configure the Lifetime value object
        builder.OwnsOne(l => l.Lifetime, lifetime => {
            lifetime.Property(l => l.StartDate).HasColumnName("LifetimeStart");
            lifetime.Property(l => l.EndDate).HasColumnName("LifetimeEnd");
        });

        // Configure the optional one-to-many relationship with parent and child locations
        builder.HasOne(l => l.ParentLocation)
            .WithMany(p => p.ChildLocations)
            .HasForeignKey("_parentLocationId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property("_parentLocationId")
            .HasColumnName("ParentLocationId");

        // Configure the optional relationship with Contract
        builder.HasOne(l => l.Contract)
            .WithMany(c => c.Locations)
            .HasForeignKey("_contractId")
            .IsRequired(false);

        builder.Property<string>("_contractId")
            .HasColumnName("ContractId")
            .HasMaxLength(12);// Adjust the length as needed

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(36);
        
        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(36);

    }
}
