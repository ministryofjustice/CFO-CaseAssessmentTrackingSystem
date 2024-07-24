using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class RiskEntityTypeConfiguration : IEntityTypeConfiguration<Risk>
{
    public void Configure(EntityTypeBuilder<Risk> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.Risk,
            DatabaseConstants.Schemas.Participant
        );

        builder.HasKey(x => x.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantId);

        builder.Property(x => x.RiskToChildren)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToKnownAdult)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToOtherPrisoners)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToPublic)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToSelf)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToStaff)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.MappaCategory)
            .HasConversion(
                x => x!.Value,
                x => MappaCategory.FromValue(x)
            );

        builder.Property(x => x.MappaLevel)
            .HasConversion(
                x => x!.Value,
                x => MappaLevel.FromValue(x)
            );

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }

}
