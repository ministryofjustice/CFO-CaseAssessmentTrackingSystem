using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class OutcomeQualityDipSampleEntityTypeConfiguration : IEntityTypeConfiguration<OutcomeQualityDipSample>
{
    public void Configure(EntityTypeBuilder<OutcomeQualityDipSample> builder)
    {
        builder.ToTable(nameof(OutcomeQualityDipSample), DatabaseConstants.Schemas.Mi);

        builder.HasKey(ds => ds.Id);

        builder.Property(ds => ds.Id)
            .ValueGeneratedNever();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ds => ds.ReviewedBy);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ds => ds.VerifiedBy);

        builder.HasOne<Contract>()
            .WithMany()
            .HasForeignKey(ds => ds.ContractId);

        builder.Property(ds => ds.Status)
            .HasConversion(
                x => x!.Value,
                x => DipSampleStatus.FromValue(x)
            ).IsRequired();

        builder.HasMany(ds => ds.Participants)
            .WithOne() // no navigation back
            .HasForeignKey(ds => ds.DipSampleId) 
            .OnDelete(DeleteBehavior.Cascade);

    }
}
