using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.ExternalRisk;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantRiskUpdateConfiguration : IEntityTypeConfiguration<ParticipantRiskUpdate>
{
    public void Configure(EntityTypeBuilder<ParticipantRiskUpdate> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.ParticipantRiskUpdate, DatabaseConstants.Schemas.Risk);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ParticipantId)
            .HasMaxLength(9)
            .IsRequired();

        builder.Property(x => x.HmppsId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DateTime)
            .IsRequired();

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ParticipantId, x.DateTime });

        builder.Ignore(x => x.DomainEvents);
    }
}
