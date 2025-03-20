using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Inductions;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Inductions;

public class WingInductionEntityTypeConfiguration : IEntityTypeConfiguration<WingInduction>
{
    public void Configure(EntityTypeBuilder<WingInduction> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.WingInduction, 
            DatabaseConstants.Schemas.Induction);

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
            .OnDelete(DeleteBehavior.Restrict); ;

        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(h => h.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(h => h.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); ;

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(h => h.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict); ;

        builder.Property(h => h.Created)
            .IsRequired();

        builder.Property(h => h.OwnerId)
            .IsRequired();

        builder.Property(h => h.CreatedBy)
            .IsRequired();

        builder.OwnsMany(h => h.Phases, phase => {
            phase.WithOwner().HasForeignKey("WingInductionId");
            phase.HasKey("WingInductionId", "Number");
            phase.Property(x => x.Number)
                .ValueGeneratedNever();
                
            phase.ToTable(
                DatabaseConstants.Tables.WingInductionPhase,
                DatabaseConstants.Schemas.Induction
                );
            phase.Property(x => x.StartDate).IsRequired();
            phase.Property(x => x.CompletedDate).IsRequired(false);

            phase.Property(a => a.Status)
              .IsRequired()
              .HasConversion(
                s => s!.Value,
                s => WingInductionPhaseStatus.FromValue(s));

            phase.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.CompletedBy)
                .OnDelete(DeleteBehavior.Restrict);

            phase.Property(e => e.AbandonReason)
                .HasConversion(
                ar => ar!.Value,
                ar => WingInductionPhaseAbandonReason.FromValue(ar));

            phase.Property(p => p.AbandonJustification)
                .HasMaxLength(ValidationConstants.NotesLength);
        });
    }
}