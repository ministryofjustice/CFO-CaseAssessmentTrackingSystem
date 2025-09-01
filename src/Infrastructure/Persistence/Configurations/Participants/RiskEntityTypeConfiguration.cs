using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
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

        builder.HasOne(x => x.Participant)
            .WithMany()
            .HasForeignKey(x => x.ParticipantId);

        builder.Property(x => x.RiskToChildrenInCustody)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToKnownAdultInCustody)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToOtherPrisonersInCustody)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToPublicInCustody)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToSelfInCustody)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToStaffInCustody)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToChildrenInCommunity)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToKnownAdultInCommunity)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToPublicInCommunity)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToSelfInCommunity)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.RiskToStaffInCommunity)
            .HasConversion(
                x => x!.Value,
                x => RiskLevel.FromValue(x)
            );

        builder.Property(x => x.ReviewReason)
            .HasConversion(
                x => x!.Value,
                x => RiskReviewReason.FromValue(x)
            );

        builder.Property(x => x.IsSubjectToSHPO)
            .HasConversion(
                x => x!.Value,
                x => ConfirmationStatus.FromValue(x)
            );

        builder.Property(x => x.NSDCase)
            .HasConversion(
                x => x!.Value,
                x => ConfirmationStatus.FromValue(x)
            );
        builder.Property(x => x.RiskToSelfInCommunityNew)
            .HasConversion(
                x => x!.Value,
                x => ConfirmationStatus.FromValue(x)
            );
        builder.Property(x => x.RiskToSelfInCustodyNew)
            .HasConversion(
                x => x!.Value,
                x => ConfirmationStatus.FromValue(x)
            );

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.CompletedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(r => r.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
