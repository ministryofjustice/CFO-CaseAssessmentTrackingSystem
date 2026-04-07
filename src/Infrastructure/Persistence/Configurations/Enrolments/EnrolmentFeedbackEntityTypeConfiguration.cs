using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants.Database;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Entities.Participants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Enrolments;

internal sealed class EnrolmentFeedbackEntityTypeConfiguration
    : IEntityTypeConfiguration<EnrolmentFeedback>
{
    public void Configure(EntityTypeBuilder<EnrolmentFeedback> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.EnrolmentFeedback,
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
        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.Property(x => x.RecipientUserId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(x => x.EnrolmentProcessedDate)
            .IsRequired();

        builder.Property(x => x.IsRead)
            .IsRequired();

        builder.Property(x => x.ReadAt)
            .IsRequired(false);

        builder.Property(x => x.EnrolmentFeedbackReason)
            .IsRequired()
            .HasMaxLength(50);
        
        // --------------------
        // SmartEnum mappings 
        // --------------------

        builder.Property(x => x.Outcome)
            .IsRequired()
            .HasConversion(
                o => o.Value,
                o => FeedbackOutcome.FromValue(o));

        builder.Property(x => x.Stage)
            .IsRequired()
            .HasConversion(
                s => s.Value,
                s => FeedbackStage.FromValue(s));

        // --------------------
        // Relationships
        // --------------------
        builder.HasOne(x => x.Participant)
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RecipientUser)
            .WithMany()
            .HasForeignKey(x => x.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);
        
        // --------------------
        // Owner / Editor 
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
        // Indexes 
        // --------------------

        builder.HasIndex(x => new { x.RecipientUserId, x.IsRead });
        builder.HasIndex(x => x.TenantId);
    }
}