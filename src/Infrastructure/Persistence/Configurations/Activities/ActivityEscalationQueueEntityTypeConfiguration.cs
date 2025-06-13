using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Activities
{
    public class ActivityEscalationQueueEntityTypeConfiguration : IEntityTypeConfiguration<ActivityEscalationQueueEntry>
    {
        public void Configure(EntityTypeBuilder<ActivityEscalationQueueEntry> builder)
        {
            builder.ToTable(DatabaseConstants.Tables.ActivityEscalationQueue, DatabaseConstants.Schemas.Activities);

            builder.Property(p => p.ActivityId)
                    .IsRequired()
                    .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            builder.Property(p => p.TenantId)
                .IsRequired()
                .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

            builder.OwnsMany(p => p.Notes, note =>
            {
                note.WithOwner();
                note.ToTable(
                $"Escalation{DatabaseConstants.Tables.Note}",
                DatabaseConstants.Schemas.Activities
                );
                note.HasKey("Id");
                note.Property(x => x.Message)
                    .HasMaxLength(ValidationConstants.NotesLength);

                note.Property(x => x.CallReference)
                    .HasMaxLength(DatabaseConstants.FieldLengths.CallReference);

                note.Property(x => x.TenantId)
                    .IsRequired()
                    .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

                note.HasOne(n => n.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedBy);

                note.Property(n => n.CreatedBy)
                    .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

                note.HasOne(n => n.LastModifiedByUser)
                    .WithMany()
                    .HasForeignKey(x => x.LastModifiedBy);

                note.Property(n => n.LastModifiedBy)
                    .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            });

            builder.HasOne(t => t.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId);

            builder.HasOne(t => t.Activity)
                .WithMany()
                .HasForeignKey(t => t.ActivityId);

            builder.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

            builder.Property(e => e.SupportWorkerId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId)
            .IsRequired();

            builder.Property(e => e.OriginalPQASubmissionDate)
                .IsRequired()
                .HasColumnType("date");           
        }
    }
}