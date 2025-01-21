using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Entities.PRIs;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.PRIs;

public class PRIEntityTypeConfiguration : IEntityTypeConfiguration<PRI>
{
    public void Configure(EntityTypeBuilder<PRI> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.PRIs,
            DatabaseConstants.Schemas.PRI
        );

        builder.HasKey(p => p.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(p => p.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.ExpectedReleaseRegion)
            .WithMany()
            .HasForeignKey(p => p.ExpectedReleaseRegionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.AssignedTo)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
