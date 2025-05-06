using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantEnrolmentHistoryConfiguration
    : IEntityTypeConfiguration<ParticipantEnrolmentHistory>
{
    public void Configure(EntityTypeBuilder<ParticipantEnrolmentHistory> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.EnrolmentHistory, 
            DatabaseConstants.Schemas.Participant
        );

        builder.HasKey(peh => peh.Id);
        builder.Property(peh => peh.ParticipantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId)
            .IsRequired();

        builder.HasIndex(peh => peh.ParticipantId);
        
        builder.Property(peh => peh.EnrolmentStatus)
            .IsRequired()
            .HasConversion(
                es => es!.Value,
                es => EnrolmentStatus.FromValue(es)
        );

        builder.Property(peh => peh.Reason)
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(peh => peh.AdditionalInformation)
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}