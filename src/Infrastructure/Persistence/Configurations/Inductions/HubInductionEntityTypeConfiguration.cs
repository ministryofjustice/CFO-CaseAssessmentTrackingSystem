using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Inductions;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Inductions;

public class HubInductionEntityTypeConfiguration : IEntityTypeConfiguration<HubInduction>
{
    public void Configure(EntityTypeBuilder<HubInduction> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.HubInduction, DatabaseConstants.Schemas.Induction);
        builder.HasKey(x => x.Id)
            .IsClustered(false);

        builder.HasIndex(x => new { x.ParticipantId, x.Created })
                .IsClustered(true);

        builder.Property(x => x.ParticipantId).IsRequired()
            .HasMaxLength(9);

        builder.Property(x => x.LocationId)
            .IsRequired();

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(h => h.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);;

        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(h => h.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(h => h.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);;

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(h => h.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);;

        builder.Property(h => h.Created)
            .IsRequired();

        builder.Property(h => h.OwnerId)
            .IsRequired();

        builder.Property (h => h.CreatedBy)
            .IsRequired();

    }
}
