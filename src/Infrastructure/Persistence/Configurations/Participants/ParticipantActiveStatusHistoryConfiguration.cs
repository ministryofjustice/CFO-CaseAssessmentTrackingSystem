using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantActiveStatusHistoryConfiguration
    : IEntityTypeConfiguration<ParticipantActiveStatusHistory>

{
    public void Configure(EntityTypeBuilder<ParticipantActiveStatusHistory> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.ActiveStatusHistory,
            DatabaseConstants.Schemas.Participant
        );

        builder.HasKey(x => x.Id);

        builder.Property(x => x.From).IsRequired();
        builder.Property(x => x.To).IsRequired();
        builder.Property(x => x.OccurredOn).IsRequired();

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantId);

        builder.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
