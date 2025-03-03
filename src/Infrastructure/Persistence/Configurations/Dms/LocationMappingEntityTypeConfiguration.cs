using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Dms;

public  class LocationMappingEntityTypeConfiguration : IEntityTypeConfiguration<LocationMapping>
{
    public void Configure(EntityTypeBuilder<LocationMapping> builder)
    {

        builder.ToTable(
            DatabaseConstants.Tables.LocationMapping,
            DatabaseConstants.Schemas.Dms
        );

        // Configure the primary key
        builder.HasKey(l => new { l.Code, l.CodeType });

        // Configure properties
        builder.Property(c => c.Code)
            .HasMaxLength(4);

        builder.Property(c => c.CodeType)
            .HasMaxLength(9);

        builder.Property(l => l.Description)
            .HasMaxLength(200);// Adjust the length as needed
        builder.Property(l => l.DeliveryRegion)
            .HasMaxLength(200)
            .IsRequired(false); ;// Adjust the length as needed


        // Configure the optional relationship with Location
        builder.HasOne(l => l.Location)
            .WithMany(c => c.LocationMappings)
            .HasForeignKey("_locationId");

        builder.Property("_locationId")
            .HasColumnName("LocationId")
            .IsRequired(false);

    }
}