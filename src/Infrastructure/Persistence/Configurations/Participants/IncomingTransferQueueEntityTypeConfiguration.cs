﻿using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class IncomingTransferQueueEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantIncomingTransferQueueEntry>
{
    public void Configure(EntityTypeBuilder<ParticipantIncomingTransferQueueEntry> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.IncomingTransferQueue, 
            DatabaseConstants.Schemas.Participant);

        builder.Property(q => q.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

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

        builder.HasIndex(q => q.Completed);
    }
}
