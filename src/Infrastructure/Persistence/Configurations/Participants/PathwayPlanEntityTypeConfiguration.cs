using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class PathwayPlanEntityTypeConfiguration
    : IEntityTypeConfiguration<PathwayPlan>

{
    public void Configure(EntityTypeBuilder<PathwayPlan> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.PathwayPlan,
            DatabaseConstants.Schemas.Participant);

        builder.HasKey(pathwayPlan => pathwayPlan.Id);

        builder.Property(pathwayPlan => pathwayPlan.ParticipantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId)
            .IsRequired();

        builder.HasIndex(pathwayPlan => pathwayPlan.ParticipantId);

        builder.Property(pathwayPlan => pathwayPlan.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(pathwayPlan => pathwayPlan.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.OwnsMany(pathwayPlan => pathwayPlan.ReviewHistories, history =>
        {
            history.ToTable(
                DatabaseConstants.Tables.PathwayPlanReviewHistory,
                DatabaseConstants.Schemas.Participant);

            history.WithOwner()
                .HasForeignKey(h => h.PathwayPlanId);

            history.HasKey(h => h.Id);

            history.Property(h => h.CreatedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            history.Property(h => h.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        });

        builder.Navigation(plan => plan.ReviewHistories).AutoInclude();

        builder.HasMany(pathwayPlan => pathwayPlan.Objectives)
            .WithOne() // no navigation back
            .HasForeignKey(objective => objective.PathwayPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(plan => plan.Objectives).AutoInclude();
    }
}