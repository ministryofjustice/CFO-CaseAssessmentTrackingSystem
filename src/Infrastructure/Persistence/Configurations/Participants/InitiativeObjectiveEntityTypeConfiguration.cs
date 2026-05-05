using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class InitiativeObjectiveEntityTypeConfiguration : IEntityTypeConfiguration<InitiativeObjective>
{
    public void Configure(EntityTypeBuilder<InitiativeObjective> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.InitiativeObjective, DatabaseConstants.Schemas.Participant);

        builder.HasKey(io => io.ObjectiveId);

        builder.Property(io => io.ObjectiveId).ValueGeneratedNever();
        builder.Property(io => io.InitiativeId).IsRequired();
        builder.Property(io => io.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.HasOne<Initiative>()
            .WithMany()
            .HasForeignKey(io => io.InitiativeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
