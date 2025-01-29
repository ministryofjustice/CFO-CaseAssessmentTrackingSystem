using Cfo.Cats.Application.Common.Validators;
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
            DatabaseConstants.Tables.PRI,
            DatabaseConstants.Schemas.PRI
        );

        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Participant)
            .WithMany()
            .HasForeignKey(p => p.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ExpectedReleaseRegion)
            .WithMany()
            .HasForeignKey(p => p.ExpectedReleaseRegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CustodyLocation)
            .WithMany()
            .HasForeignKey(p => p.CustodyLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.AssignedTo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.AssignedTo)
            .HasMaxLength(36);

        builder.Property(x => x.ReasonCommunityDidNotAttendInPerson).HasMaxLength(ValidationConstants.NotesLength);
        builder.Property(x => x.ReasonCustodyDidNotAttendInPerson).HasMaxLength(ValidationConstants.NotesLength);
        builder.Property(x => x.ReasonParticipantDidNotAttendInPerson).HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
