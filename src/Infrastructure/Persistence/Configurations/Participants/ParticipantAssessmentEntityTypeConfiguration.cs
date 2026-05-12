using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.ValueObjects;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantAssessmentEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantAssessment>
{
    public void Configure(EntityTypeBuilder<ParticipantAssessment> builder)
    {
        builder.ToTable(
        DatabaseConstants.Tables.Assessment, 
        DatabaseConstants.Schemas.Participant);

        builder.HasKey(t => t.Id);

        builder.HasOne<Domain.Entities.Participants.Participant>()
            .WithMany()
            .HasForeignKey(pa => pa.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ParticipantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(pa => pa.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(pa => pa.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.OwnsMany(p => p.Scores, score => {
            score.WithOwner().HasForeignKey("AssessmentId");
            score.HasKey("AssessmentId", "Pathway");
            score.ToTable(
            DatabaseConstants.Tables.AssessmentPathwayScore, 
            DatabaseConstants.Schemas.Participant
            );
            score.Property(x => x.Pathway).HasMaxLength(50).IsRequired();
            score.Property(x => x.Score).HasColumnType("float").IsRequired();
        });

        builder.OwnsMany(p => p.Answers, answer => {
            answer.WithOwner().HasForeignKey("AssessmentId");
            answer.HasKey("Id");
            answer.HasIndex("AssessmentId", nameof(AssessmentAnswer.QuestionCode), nameof(AssessmentAnswer.Answer)).IsUnique();
            answer.ToTable(
                DatabaseConstants.Tables.AssessmentAnswer,
                DatabaseConstants.Schemas.Participant
            );
            answer.Property(x => x.QuestionCode).HasMaxLength(3).IsRequired();
            answer.Property(x => x.Answer).HasMaxLength(80).IsRequired();
        });
            
        builder.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.CompletedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}