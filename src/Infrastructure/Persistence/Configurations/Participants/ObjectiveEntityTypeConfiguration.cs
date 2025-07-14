using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ObjectiveEntityTypeConfiguration : IEntityTypeConfiguration<Objective>
{
    public void Configure(EntityTypeBuilder<Objective> objective)
    {
        objective.ToTable(
        DatabaseConstants.Tables.Objective,
        DatabaseConstants.Schemas.Participant);

        objective.HasKey(o => o.Id);

        objective.Property(o => o.Id).ValueGeneratedNever();

        objective.Property(o => o.PathwayPlanId).IsRequired();

        objective.Property(o => o.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        objective.Property(o => o.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        objective.Property(o => o.CompletedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        objective.HasOne(o => o.CreatedByUser)
            .WithMany()
            .HasForeignKey(o => o.CreatedBy);

        objective.HasOne(o => o.CompletedByUser)
            .WithMany()
            .HasForeignKey(o => o.CompletedBy);

        objective.Property(o => o.CompletedStatus)
            .HasConversion(
            x => x!.Value,
            x => CompletionStatus.FromValue(x));

        objective.HasMany(o => o.Tasks)
            .WithOne() // no navigation back
            .HasForeignKey(t => t.ObjectiveId)
            .OnDelete(DeleteBehavior.Cascade);

        objective.Navigation(o => o.Tasks).AutoInclude();
    }
}