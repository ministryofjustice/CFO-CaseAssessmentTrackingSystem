﻿using Amazon.Runtime.Documents;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.ValueObjects;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class ParticipantEntityTypeConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable(DatabaseSchema.Tables.Participant);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasMaxLength(9)
            .ValueGeneratedNever();

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.MiddleName)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.Property(p => p.ReferralSource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ReferralComments)
            .IsRequired(false);

        builder.Property(e => e.EnrolmentStatus)
            .IsRequired()
            .HasConversion(
            es => es!.Value,
            es => EnrolmentStatus.FromValue(es)
            );

        builder.Property(e => e.ConsentStatus)
            .IsRequired()
            .HasConversion(
            cs => cs!.Value,
            cs => ConsentStatus.FromValue(cs));

        builder.HasOne(e => e.CurrentLocation)
            .WithMany()
            .HasForeignKey("_currentLocationId")
            .HasConstraintName("FK_Participant_Location");
        
        builder.Property<int>("_currentLocationId")
            .HasColumnName("CurrentLocationId");
        
        builder.HasOne(e => e.EnrolmentLocation)
            .WithMany()
            .HasForeignKey("_enrolmentLocationId")
            .HasConstraintName("FK_Participant_EnrolmentLocation");

        builder.Property<int?>("_enrolmentLocationId")
            .HasColumnName("EnrolmentLocationId");

        builder.OwnsMany(participant => participant.Consents, consent => {
            consent.WithOwner()
                .HasForeignKey("ParticipantId");

            consent.ToTable(DatabaseSchema.Tables.Consent);

            consent.OwnsOne(p => p.Lifetime, lt => {
                lt.Property(t => t.StartDate).IsRequired()
                    .HasColumnName("ValidFrom");
                lt.Property(t => t.EndDate)
                    .IsRequired()
                    .HasColumnName("ValidTo");
            });
                

            consent.HasOne(c => c.Document)
                .WithMany()
                .HasForeignKey("_documentId");

            consent.Property("_documentId")
                .HasColumnName("DocumentId");
        });
        
        builder.OwnsMany(c => c.RightToWorks, a => {
            a.WithOwner()
                .HasForeignKey("ParticipantId");

            a.ToTable(DatabaseSchema.Tables.RightToWork);

            a.OwnsOne(p => p.Lifetime, lt => {
                
                lt.Property(t => t.StartDate)
                    .IsRequired()
                    .HasColumnName("ValidFrom");
                
                lt.Property(t => t.EndDate)
                    .IsRequired()
                    .HasColumnName("ValidTo");
                
                a.HasOne(c => c.Document)
                    .WithMany()
                    .HasForeignKey("_documentId");

                a.Property("_documentId")
                    .HasColumnName("DocumentId");
                
            });
            
        });

        builder.Navigation(p => p.Consents)
            .AutoInclude();

        builder.Navigation(p => p.EnrolmentLocation)
            .AutoInclude();
        
        builder.Navigation(p => p.CurrentLocation)
            .AutoInclude();

        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId);
        
        builder.Property(c => c.CreatedBy)
            .HasMaxLength(36);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(36);

        builder.Property(p => p.OwnerId)
            .HasMaxLength(36);

    }
}
