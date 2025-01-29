using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.AccessControl;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class PathwayPlanEntityTypeConfiguration
    : IEntityTypeConfiguration<PathwayPlan>

{
    public void Configure(EntityTypeBuilder<PathwayPlan> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.PathwayPlan,
            DatabaseConstants.Schemas.Participant
        );

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
                .HasForeignKey(history => history.PathwayPlanId);

            history.HasKey(history => history.Id);

            history.Property(history => history.CreatedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            history.Property(history => history.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        });

        builder.Navigation(objective => objective.ReviewHistories).AutoInclude();

        builder.OwnsMany(pathwayPlan => pathwayPlan.Objectives, objective =>
        {
            objective.ToTable(
                DatabaseConstants.Tables.Objective,
                DatabaseConstants.Schemas.Participant);

            objective
                .WithOwner()
                .HasForeignKey(objective => objective.PathwayPlanId);

            objective.HasKey(objective => objective.Id);

            objective.Property(objective => objective.Id)
                .ValueGeneratedNever();

            objective.Property(objective => objective.CreatedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            objective.Property(objective => objective.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            objective.Property(objective => objective.CompletedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            objective.HasOne(task => task.CompletedByUser)
                .WithMany()
                .HasForeignKey(task => task.CompletedBy);

            objective.Property(objective => objective.CompletedStatus)
                .HasConversion(
                    x => x!.Value,
                    x => CompletionStatus.FromValue(x)
                );

            objective.OwnsMany(objective => objective.Tasks, task =>
            {
                task.ToTable(
                    DatabaseConstants.Tables.ObjectiveTask,
                    DatabaseConstants.Schemas.Participant);

                task.WithOwner()
                    .HasForeignKey(task => task.ObjectiveId);

                task.HasKey(task => task.Id);

                task.Property(task => task.Id)
                    .ValueGeneratedNever();

                task.Property(task => task.CreatedBy)
                    .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

                task.Property(task => task.LastModifiedBy)
                    .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

                task.Property(task => task.CompletedBy)
                    .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

                task.HasOne(task => task.CompletedByUser)
                    .WithMany()
                    .HasForeignKey(task => task.CompletedBy);

                task.Property(task => task.CompletedStatus)
                    .HasConversion(
                        x => x!.Value,
                        x => CompletionStatus.FromValue(x)
                    );
            });

            objective.Navigation(objective => objective.Tasks).AutoInclude();
        });

        builder.Navigation(pathwayPlan => pathwayPlan.Objectives).AutoInclude();
    }
}
