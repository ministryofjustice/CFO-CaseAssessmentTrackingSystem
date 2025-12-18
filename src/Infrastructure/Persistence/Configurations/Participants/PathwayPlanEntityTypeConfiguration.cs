using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Common.Enums;
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

        builder.OwnsMany(p => p.PathwayPlanReviews, reviews =>
        {
            reviews.ToTable(
                DatabaseConstants.Tables.PathwayPlanReview,
                DatabaseConstants.Schemas.Participant);

            reviews.WithOwner()
                .HasForeignKey(r => r.PathwayPlanId);

            reviews.HasKey(r => r.Id);

            reviews.Property(r => r.ParticipantId)
                .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId)
                .IsRequired();

            reviews.Property(r => r.LocationId)
                .IsRequired();

            reviews.Property(r => r.ReviewDate)
                .IsRequired();

            reviews.Property(r => r.Review)
                .HasMaxLength(ValidationConstants.NotesLength);

            reviews.Property(r => r.ReviewReason)
                .HasConversion(
                    r => r.Value,
                    v => PathwayPlanReviewReason.FromValue(v))
                .IsRequired();

            reviews.Property(r => r.CreatedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            reviews.Property(r => r.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            reviews.HasIndex(r => r.PathwayPlanId);
            reviews.HasIndex(r => r.ParticipantId);
        });

        builder.Navigation(plan => plan.PathwayPlanReviews).AutoInclude();

        builder.HasMany(pathwayPlan => pathwayPlan.Objectives)
            .WithOne() // no navigation back
            .HasForeignKey(objective => objective.PathwayPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(plan => plan.Objectives).AutoInclude();
    }
}