using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class DipSampleParticipantEntityTypeConfiguration : IEntityTypeConfiguration<OutcomeQualityDipSampleParticipant>
{
    public void Configure(EntityTypeBuilder<OutcomeQualityDipSampleParticipant> builder)
    {
        builder.ToTable(nameof(OutcomeQualityDipSampleParticipant), DatabaseConstants.Schemas.Mi);

        builder.HasKey(dsp => dsp.Id);

        builder.HasIndex(dsp => new { dsp.DipSampleId, dsp.ParticipantId })
            .IsUnique(true);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(dsp => dsp.ParticipantId);

        builder.HasOne<OutcomeQualityDipSample>()
            .WithMany()
            .HasForeignKey(dsp => dsp.DipSampleId);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(dsp => dsp.CsoReviewedBy);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(dsp => dsp.CpmReviewedBy);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(dsp => dsp.FinalReviewedBy);

        builder.Property(dsp => dsp.LocationType)
            .HasMaxLength(64);

        builder.Property(dsp => dsp.CsoComments)
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(s => s.HasClearParticipantJourney)
            .IsRequired()
            .HasConversion(
                dsa => dsa!.Value,
                dsa => DipSampleAnswer.FromValue(dsa)
            );

        builder.Property(s => s.ShowsTaskProgression)
            .IsRequired()
            .HasConversion(
                dsa => dsa!.Value,
                dsa => DipSampleAnswer.FromValue(dsa)
            );

        builder.Property(s => s.ActivitiesLinkToTasks)
            .IsRequired()
            .HasConversion(
                dsa => dsa!.Value,
                dsa => DipSampleAnswer.FromValue(dsa)
            );

        builder.Property(s => s.TTGDemonstratesGoodPRIProcess)
            .IsRequired()
            .HasConversion(
                dsa => dsa!.Value,
                dsa => DipSampleAnswer.FromValue(dsa)
            );
        
        builder.Property(s => s.SupportsJourneyAndAlignsWithDoS)
            .IsRequired()
            .HasConversion(
                dsa => dsa!.Value,
                dsa => DipSampleAnswer.FromValue(dsa)
            );

        builder.Property(s => s.CsoIsCompliant)
            .IsRequired()
            .HasConversion(
                c => c!.Value,
                es => ComplianceAnswer.FromValue(es)
            );

        builder.Property(s => s.CpmIsCompliant)
            .IsRequired()
            .HasConversion(
                c => c!.Value,
                c => ComplianceAnswer.FromValue(c)
            );
        
        builder.Property(s => s.FinalIsCompliant)
            .IsRequired()
            .HasConversion(
                c => c!.Value,
                c => ComplianceAnswer.FromValue(c)
            );

    }
}
