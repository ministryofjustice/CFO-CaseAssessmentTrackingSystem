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

        builder.HasKey(io => io.Id);

        builder.Property(io => io.Id).ValueGeneratedNever();
        builder.Property(io => io.ObjectiveId).IsRequired();
        builder.Property(io => io.InitiativeId).IsRequired();
        builder.Property(io => io.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.Property(io => io.TenantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Property(io => io.CreatedBy).HasMaxLength(36);
        builder.Property(io => io.LastModifiedBy).HasMaxLength(36);

        builder.HasIndex(io => io.ObjectiveId).IsUnique();

        builder.HasOne<Initiative>(io => io.Initiative)
            .WithMany()
            .HasForeignKey(io => io.InitiativeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
