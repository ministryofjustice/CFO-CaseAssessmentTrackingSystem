using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class PriCodeEntityTypeConfiguration : IEntityTypeConfiguration<PriCode>
{
    public void Configure(EntityTypeBuilder<PriCode> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.PriCode,
            DatabaseConstants.Schemas.PRI);

        builder.HasKey(l => l.ParticipantId);

        builder.Ignore(l => l.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(pa => pa.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ParticipantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.Property(x => x.Created)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId)
            .IsRequired();

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}