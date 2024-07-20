﻿using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Enrolments;

public class EnrolmentQa1QueueEntityTypeConfiguration : IEntityTypeConfiguration<EnrolmentQa1QueueEntry>
{
    public void Configure(EntityTypeBuilder<EnrolmentQa1QueueEntry> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.EnrolmentQa1Queue, DatabaseConstants.Schemas.Enrolment);
        
        builder.Property(p => p.ParticipantId)
            .IsRequired()
            .HasMaxLength(9);
        
        builder.Property(p => p.TenantId)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.OwnsMany(p => p.Notes, note =>
        {
            note.WithOwner();
            note.ToTable(
            $"Qa1Queue{DatabaseConstants.Tables.Note}",
            DatabaseConstants.Schemas.Enrolment
            );
            note.HasKey("Id");
            note.Property(x => x.Message).HasMaxLength(256);

            note.Property(x => x.CallReference)
                .HasMaxLength(DatabaseConstants.FieldLengths.CallReference);
            
            note.HasOne(n => n.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy);
            
            note.Property(n => n.CreatedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            
            note.HasOne(n => n.LastModifiedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastModifiedBy);
            
            note.Property(n => n.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            
        });
        
        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(9);
        
        builder.HasOne(t => t.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId);
        
        builder.HasOne(t => t.Participant)
            .WithMany()
            .HasForeignKey(t => t.ParticipantId);

        builder.Property(e => e.TenantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);
        
    }
}