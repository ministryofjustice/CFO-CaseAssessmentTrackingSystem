using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class PathwayPlanReviewEntityTypeConfiguration
    : IEntityTypeConfiguration<PathwayPlanReview>
{       
    public void Configure(EntityTypeBuilder<PathwayPlanReview> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.PathwayPlanReview,
            DatabaseConstants.Schemas.Participant);
        
        builder.HasKey(pathwayPlanReview => pathwayPlanReview.Id);
        
        builder.Property(pathwayPlanReview => pathwayPlanReview.PathwayPlanId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId)
            .IsRequired();

        builder.HasIndex(pathwayPlanReview => pathwayPlanReview.PathwayPlanId);
        
        builder.Property(pathwayPlanReview => pathwayPlanReview.ParticipantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId)
            .IsRequired();

        builder.HasIndex(pathwayPlanReview => pathwayPlanReview.ParticipantId);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(pathwayPlanReview => pathwayPlanReview.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(pathwayPlanReview => pathwayPlanReview.ReviewDate)
            .IsRequired();
        
        builder.Property(x => x.ReviewReason)
            .HasConversion(
                x => x!.Value,
                x => PathwayPlanReviewReason.FromValue(x)
            );
        
        builder.Property(x => x.Review)
            .HasMaxLength(ValidationConstants.NotesLength);
        
        builder.Property(pathwayPlan => pathwayPlan.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(pathwayPlan => pathwayPlan.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}