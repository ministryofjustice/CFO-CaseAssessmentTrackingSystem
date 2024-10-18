using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantLocationHistoryConfiguration
    : IEntityTypeConfiguration<ParticipantLocationHistory>
{
    public void Configure(EntityTypeBuilder<ParticipantLocationHistory> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.LocationHistory,
            DatabaseConstants.Schemas.Participant
        );

        builder.HasKey(plh => plh.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(plh => plh.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(plh => plh.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(plh => plh.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(plh => plh.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
