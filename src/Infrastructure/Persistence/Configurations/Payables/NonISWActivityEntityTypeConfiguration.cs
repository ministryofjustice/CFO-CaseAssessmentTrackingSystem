﻿using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Payables;

public class NonISWActivityEntityTypeConfiguration : IEntityTypeConfiguration<NonISWActivity>
{
    public void Configure(EntityTypeBuilder<NonISWActivity> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.NonISWActivities, 
            DatabaseConstants.Schemas.Participant);

        builder.HasKey(a => a.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(a => a.ParticipantId);

        builder.Property(a => a.Definition)
            .IsRequired()
            .HasConversion(
                d => d!.Value,
                d => ActivityDefinition.FromValue(d)
            );

        builder.HasOne(a => a.TookPlaceAtLocation)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TookPlaceAtContract)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.ParticipantCurrentLocation)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.ParticipantCurrentContract)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.ParticipantStatus)
            .IsRequired()
            .HasConversion(
                ps => ps!.Value,
                ps => EnrolmentStatus.FromValue(ps)
            );

        builder.Property(a => a.AdditionalInformation)
            .HasMaxLength(1000);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(a => a.TenantId);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion(
                s => s!.Value,
                s => ActivityStatus.FromValue(s)
            );
    }
}
