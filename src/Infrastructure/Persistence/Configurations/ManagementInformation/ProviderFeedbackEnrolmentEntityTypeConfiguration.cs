using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class ProviderFeedbackEnrolmentEntityTypeConfiguration : IEntityTypeConfiguration<ProviderFeedbackEnrolment>
{
    public void Configure(EntityTypeBuilder<ProviderFeedbackEnrolment> builder)
    {
        builder.ToTable(nameof(ProviderFeedbackEnrolment), DatabaseConstants.Schemas.Mi);
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CreatedOn)
            .IsRequired();
        
        builder.Property(x => x.SourceTable)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Queue)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.QueueEntryId)
            .IsRequired();

        builder.Property(x => x.NoteId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Property(x => x.ContractId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ContractId);

        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.Property(x => x.SupportWorkerId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.ProviderQaUserId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.CfoUserId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.PqaSubmittedDate)
            .IsRequired();
        
        builder.Property(x => x.ActionDate)
            .IsRequired();

        builder.Property(x => x.NoteCreatedDate);

        builder.Property(x => x.NoteCreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.Message)
            .HasMaxLength(1000);
        
        builder.Property(x => x.FeedbackType);

        builder.HasIndex(x => new {
            x.SourceTable, 
            x.QueueEntryId,
            x.NoteId
        })
        .IsUnique()
        .HasDatabaseName("ix_ProviderFeedbackEnrolment_SourceTable_QueueEntryId_NoteId");

        builder.HasIndex(x => new { 
            x.SourceTable,
            x.TenantId,
            x.ActionDate
        })
        .HasDatabaseName("ix_ProviderFeedbackEnrolment_SourceTable_TenantId_ActionDate");       
        
    }
}
