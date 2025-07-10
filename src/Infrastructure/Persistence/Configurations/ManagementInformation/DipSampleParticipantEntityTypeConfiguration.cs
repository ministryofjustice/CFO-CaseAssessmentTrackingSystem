using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class DipSampleParticipantEntityTypeConfiguration : IEntityTypeConfiguration<DipSampleParticipant>
{
    public void Configure(EntityTypeBuilder<DipSampleParticipant> builder)
    {
        builder.ToTable(nameof(DipSampleParticipant), DatabaseConstants.Schemas.Mi);

        builder.HasKey(dsp => new { dsp.DipSampleId, dsp.ParticipantId });

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(dsp => dsp.ParticipantId);

        builder.HasOne<DipSample>()
            .WithMany()
            .HasForeignKey(dsp => dsp.DipSampleId);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(dsp => dsp.ReviewedBy);

        builder.Property(dsp => dsp.LocationType)
            .HasMaxLength(64);

        builder.Property(dsp => dsp.Remarks)
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.HasIndex(dsp => dsp.IsCompliant);
    }
}
