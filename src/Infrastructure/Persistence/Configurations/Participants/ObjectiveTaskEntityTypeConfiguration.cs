using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ObjectiveTaskEntityTypeConfiguration : IEntityTypeConfiguration<ObjectiveTask>
{
    public void Configure(EntityTypeBuilder<ObjectiveTask> task)
    {
        task.ToTable(
        DatabaseConstants.Tables.ObjectiveTask,
        DatabaseConstants.Schemas.Participant);

        task.HasKey(t => t.Id);

        task.Property(t => t.Id).ValueGeneratedNever();

        task.Property(t => t.ObjectiveId).IsRequired();

        task.Property(t => t.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        task.Property(t => t.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        task.Property(t => t.CompletedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        task.HasOne(t => t.CompletedByUser)
            .WithMany()
            .HasForeignKey(t => t.CompletedBy);

        task.Property(t => t.CompletedStatus)
            .HasConversion(
            x => x!.Value,
            x => CompletionStatus.FromValue(x));
    }
}