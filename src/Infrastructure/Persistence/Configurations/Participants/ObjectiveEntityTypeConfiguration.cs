using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ObjectiveEntityTypeConfiguration
    : IEntityTypeConfiguration<Objective>

{
    public void Configure(EntityTypeBuilder<Objective> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.Objective,
            DatabaseConstants.Schemas.Participant
        );

        builder.HasKey(o => o.Id);

        builder.Property(o => o.ParticipantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId)
            .IsRequired();

        builder.HasIndex(o => o.ParticipantId);

        builder.Property(o => o.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(o => o.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.OwnsMany(o => o.Tasks, task =>
        {
            task.WithOwner();
            task.ToTable(
                DatabaseConstants.Tables.ObjectiveTask, 
                DatabaseConstants.Schemas.Participant);

            task.HasKey(t => t.Id);

            task.Property(t => t.CreatedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            task.Property(t => t.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            task.Property(t => t.CompletedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            task.HasOne(t => t.CompletedByUser)
                .WithMany()
                .HasForeignKey(t => t.CompletedBy);
        });
    }
}
