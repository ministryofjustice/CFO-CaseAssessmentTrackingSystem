using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants.Database;
using Cfo.Cats.Application.Common.Validators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Activities;

internal sealed class ActivityFeedbackEntityTypeConfiguration
    : IEntityTypeConfiguration<ActivityFeedback>
{
    public void Configure(EntityTypeBuilder<ActivityFeedback> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.ActivityFeedback,
            DatabaseConstants.Schemas.Activities);

        // --------------------
        // Key
        // --------------------

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        // --------------------
        // Core properties
        // --------------------

        builder.Property(x => x.ActivityId)
            .IsRequired();

        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.Property(x => x.RecipientUserId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(x => x.ActivityProcessedDate)
            .IsRequired(false);

        builder.Property(x => x.IsRead)
            .IsRequired();

        builder.Property(x => x.ReadAt)
            .IsRequired(false);

        // --------------------
        // SmartEnum mappings (same pattern as Participant)
        // --------------------

        builder.Property(x => x.OutCome)
            .IsRequired(false)
            .HasConversion(
                o => o!.Value,
                o => FeedbackOutcome.FromValue(o));

        builder.Property(x => x.Stage)
            .IsRequired(false)
            .HasConversion(
                s => s!.Value,
                s => FeedbackStage.FromValue(s));

        // --------------------
        // Relationships
        // --------------------

        builder.HasOne(x => x.Activity)
            .WithMany()
            .HasForeignKey(x => x.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Participant)
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RecipientUser)
            .WithMany()
            .HasForeignKey(x => x.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // --------------------
        // Owner / Editor (mirror Participant)
        // --------------------

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId);

        builder.HasOne(x => x.Editor)
            .WithMany()
            .HasForeignKey(x => x.EditorId);

        builder.Property(x => x.OwnerId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.EditorId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        // --------------------
        // Auditing
        // --------------------

        builder.Property(x => x.Created)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        // --------------------
        // Indexes (same philosophy)
        // --------------------

        builder.HasIndex(x => x.ActivityId);
        builder.HasIndex(x => new { x.RecipientUserId, x.IsRead });
    }
}