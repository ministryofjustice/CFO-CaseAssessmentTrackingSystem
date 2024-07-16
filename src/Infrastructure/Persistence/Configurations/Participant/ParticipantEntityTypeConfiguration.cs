using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participant;

public class ParticipantEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.Participants.Participant>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Participants.Participant> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.Participant, 
            DatabaseConstants.Schemas.Participant
        );

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId)
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

            consent.ToTable(
                DatabaseConstants.Tables.Consent, 
                DatabaseConstants.Schemas.Participant);

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

            consent.Property(x => x.CreatedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            consent.Property(x => x.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        });
        
        builder.OwnsMany(c => c.RightToWorks, a => {
            a.WithOwner()
                .HasForeignKey("ParticipantId");

            a.ToTable(
                DatabaseConstants.Tables.RightToWork, 
                DatabaseConstants.Schemas.Participant);
            
            a.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            a.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

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

        builder.OwnsMany(p => p.Notes, note =>
        {
            note.WithOwner();
            note.ToTable(
                DatabaseConstants.Tables.Note, 
                DatabaseConstants.Schemas.Participant
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
            
            note.HasOne(n => n.ModifiedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastModifiedBy);
            
            note.Property(n => n.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            
        });

        builder.Navigation(p => p.Consents)
            .AutoInclude();

        builder.Navigation(p => p.EnrolmentLocation)
            .AutoInclude();
        
        builder.Navigation(p => p.CurrentLocation)
            .AutoInclude();

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId);
        
        builder.HasOne(x => x.Editor)
            .WithMany()
            .HasForeignKey(x => x.EditorId);

        builder.Property(x => x.OwnerId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder.Property(x => x.EditorId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

    }
}
