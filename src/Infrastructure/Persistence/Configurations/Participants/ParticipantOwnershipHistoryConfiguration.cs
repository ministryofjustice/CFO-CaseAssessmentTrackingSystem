using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantOwnershipHistoryConfiguration
    : IEntityTypeConfiguration<ParticipantOwnershipHistory>
{
    public void Configure(EntityTypeBuilder<ParticipantOwnershipHistory> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.OwnershipHistory,
            DatabaseConstants.Schemas.Participant
        );

        builder.HasKey(plh => plh.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(plh => plh.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(plh => plh.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(plh => plh.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(plh => plh.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
