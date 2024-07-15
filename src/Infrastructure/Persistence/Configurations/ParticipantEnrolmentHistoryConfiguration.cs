using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class ParticipantEnrolmentHistoryConfiguration
    : IEntityTypeConfiguration<ParticipantEnrolmentHistory>
{
    public void Configure(EntityTypeBuilder<ParticipantEnrolmentHistory> builder)
    {
        builder.ToTable(DatabaseSchema.Tables.ParticipantEnrolmentHistory);

        builder.HasKey(peh => peh.Id);
        builder.Property(peh => peh.ParticipantId)
            .HasMaxLength(9)
            .IsRequired();

        builder.HasIndex(peh => peh.ParticipantId);
        
        builder.Property(peh => peh.EnrolmentStatus)
            .IsRequired()
            .HasConversion(
                es => es!.Value,
                es => EnrolmentStatus.FromValue(es)
            );
        
        builder.Property(c => c.CreatedBy)
            .HasMaxLength(36);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(36);
    }
}
