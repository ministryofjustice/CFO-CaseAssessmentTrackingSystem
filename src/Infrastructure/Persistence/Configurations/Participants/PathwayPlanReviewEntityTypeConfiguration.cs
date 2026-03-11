using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class PathwayPlanReviewEntityTypeConfiguration
    : IEntityTypeConfiguration<PathwayPlanReview>
{
    public void Configure(EntityTypeBuilder<PathwayPlanReview> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.PathwayPlanReview,
            DatabaseConstants.Schemas.Participant);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ParticipantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId)
            .IsRequired();

        builder.Property(r => r.LocationId)
            .IsRequired();

        builder.Property(r => r.ReviewDate)
            .IsRequired();

        builder.Property(r => r.Review)
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(r => r.ReviewReason)
            .HasConversion(
                r => r.Value,
                v => PathwayPlanReviewReason.FromValue(v))
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(r => r.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.HasIndex(r => r.PathwayPlanId);
        builder.HasIndex(r => r.ParticipantId);
    }
}