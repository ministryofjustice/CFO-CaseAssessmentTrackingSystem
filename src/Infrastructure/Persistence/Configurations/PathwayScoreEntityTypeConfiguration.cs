using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations
{
    public class ParticipantAssessmentEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantAssessment>
    {
        public void Configure(EntityTypeBuilder<ParticipantAssessment> builder)
        {
            builder.ToTable(DatabaseSchema.Tables.ParticipantAssessment);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasMaxLength(9);

            builder.HasOne<Participant>()
                .WithMany()
                .HasForeignKey(pa => pa.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(pa => pa.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.OwnsMany(p => p.Scores, score => {
                score.WithOwner().HasForeignKey("AssessmentId");
                score.HasKey("AssessmentId", "Pathway");
                score.ToTable(DatabaseSchema.Tables.ParticipantAssessmentPathwayScore);
                score.Property(x => x.Pathway).HasMaxLength(50).IsRequired();
                score.Property(x => x.Score).HasColumnType("float").IsRequired();
            });
            

        }
    }
}