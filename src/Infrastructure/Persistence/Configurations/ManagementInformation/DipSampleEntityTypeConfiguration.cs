using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class DipSampleEntityTypeConfiguration : IEntityTypeConfiguration<OutcomeQualityDipSample>
{
    public void Configure(EntityTypeBuilder<OutcomeQualityDipSample> builder)
    {
        builder.ToTable(nameof(OutcomeQualityDipSample), DatabaseConstants.Schemas.Mi);

        builder.HasKey(ds => ds.Id);

        builder.Property(ds => ds.Id)
            .ValueGeneratedNever();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ds => ds.CompletedBy);

        builder.HasOne<Contract>()
            .WithMany()
            .HasForeignKey(ds => ds.ContractId);

        builder.Property(ds => ds.Status)
            .HasConversion(
                x => x!.Value,
                x => DipSampleStatus.FromValue(x)
            ).IsRequired();
    }
}
