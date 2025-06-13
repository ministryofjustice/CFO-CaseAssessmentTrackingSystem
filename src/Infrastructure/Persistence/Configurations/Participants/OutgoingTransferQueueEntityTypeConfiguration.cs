using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class OutgoingTransferQueueEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantOutgoingTransferQueueEntry>
{
    public void Configure(EntityTypeBuilder<ParticipantOutgoingTransferQueueEntry> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.OutgoingTransferQueue,
            DatabaseConstants.Schemas.Participant);

        builder.Property(q => q.TransferType)
            .IsRequired()
            .HasConversion(
                tlt => tlt!.Value,
                tlt => TransferLocationType.FromValue(tlt)
            );

        builder.HasOne(q => q.ToContract)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.FromContract)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.ToLocation)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.FromLocation)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(q => q.IsReplaced);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(q => q.PreviousOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(q => q.PreviousTenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Participant)
            .WithMany()
            .HasForeignKey(q => q.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(q => q.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(q => q.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
