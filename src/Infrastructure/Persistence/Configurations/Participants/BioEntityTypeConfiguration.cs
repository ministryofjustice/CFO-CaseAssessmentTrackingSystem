using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Bios;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Bios
{
    public class BioEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantBio>
    {
        public void Configure(EntityTypeBuilder<ParticipantBio> builder)
        {
            builder.ToTable(
                DatabaseConstants.Tables.Bio,
                DatabaseConstants.Schemas.Participant);

            builder.HasKey(t => t.Id);

            builder.HasOne<Domain.Entities.Participants.Participant>()
                .WithMany()
                .HasForeignKey(pa => pa.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ParticipantId)
                .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);
            builder.Property(x => x.Status)
                .HasConversion(
                    x => x!.Value,
                    x => BioStatus.FromValue(x)
                );

            builder.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            builder.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            builder.Property(x => x.CompletedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        }
    }
}