using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantEntityTypeConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
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

        builder.Property(p => p.Gender)
            .HasMaxLength(29);

        builder.Property(p => p.Nationality)
            .HasMaxLength(50);

        builder.Property(p => p.ReferralSource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ReferralComments)
            .IsRequired(false)
            .HasMaxLength(ValidationConstants.NotesLength); 

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

        builder.Property(e => e.RiskDueReason)
            .IsRequired()
            .HasConversion(
            cs => cs!.Value,
            cs => RiskDueReason.FromValue(cs));

        builder.Property(p => p.DateOfFirstConsent)
            .IsRequired(false);      

        builder.Property(p => p.RiskDue)
            .IsRequired(false)
            .HasColumnType("date");

        builder.Property(x => x.PrimaryRecordKeyAtCreation)
            .HasMaxLength(DatabaseConstants.FieldLengths.ExternalIdentifier);

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

        builder.Property(p => p.EnrolmentLocationJustification)
            .HasMaxLength(ValidationConstants.NotesLength);

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

        builder.OwnsMany(p => p.ExternalIdentifiers, externalIdentifier =>
        {
            externalIdentifier.WithOwner();

            externalIdentifier.ToTable(
                DatabaseConstants.Tables.ExternalIdentifier,
                DatabaseConstants.Schemas.Participant
            );

            externalIdentifier.HasKey(
                "Value", 
                "Type", 
                "ParticipantId"
            );

            externalIdentifier.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(DatabaseConstants.FieldLengths.ExternalIdentifier);

            externalIdentifier.Property(e => e.Type)
                .IsRequired()
                .HasConversion(
                    id => id!.Value,
                    id => ExternalIdentifierType.FromValue(id)
                );
        });

        builder.OwnsMany(p => p.Notes, note =>
        {
            note.WithOwner();
            note.ToTable(
                DatabaseConstants.Tables.Note, 
                DatabaseConstants.Schemas.Participant
            );
            note.HasKey("Id");
            note.Property(x => x.Message).HasMaxLength(ValidationConstants.NotesLength);

            note.Property(x => x.CallReference)
                .HasMaxLength(DatabaseConstants.FieldLengths.CallReference);

            note.Property(x => x.TenantId)
                .IsRequired()
                .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

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

        builder.OwnsOne(p => p.Supervisor, supervisor =>
        {
            supervisor.WithOwner();
            supervisor.ToTable(
                DatabaseConstants.Tables.Supervisor,
                DatabaseConstants.Schemas.Participant
            );

            supervisor.HasKey(s => s.Id);

            supervisor.Property(s => s.Name)
                .HasMaxLength(128);

            supervisor.Property(s => s.EmailAddress)
                .HasMaxLength(100);

            supervisor.Property(s => s.TelephoneNumber)
                .HasMaxLength(16);

            supervisor.Property(s => s.MobileNumber)
                .HasMaxLength(16);

            supervisor.Property(s => s.Address)
                .HasMaxLength(256);

            supervisor.Property(s => s.CreatedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

            supervisor.Property(s => s.LastModifiedBy)
                .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        });

        builder.OwnsOne(p => p.PersonalDetail, personalDetails =>
        {
            personalDetails.WithOwner();
            personalDetails.ToTable(
                DatabaseConstants.Tables.PersonalDetails,
                DatabaseConstants.Schemas.Participant
            );

            personalDetails.HasKey(s => s.Id);

            personalDetails.Property(s => s.PreferredNames)
                .HasMaxLength(128);

            personalDetails.Property(s => s.PreferredPronouns)
                .HasMaxLength(16);

            personalDetails.Property(s => s.PreferredTitle)
                .HasMaxLength(6);

            personalDetails.Property(s => s.NINo)
                .HasMaxLength(9);

            personalDetails.Property(s => s.AdditionalNotes)
                .HasMaxLength(256);
        });

        builder.Navigation(p => p.Consents)
            .AutoInclude();

        builder.Navigation(p => p.EnrolmentLocation)
            .AutoInclude();
        
        builder.Navigation(p => p.CurrentLocation)
            .AutoInclude();

        builder.Navigation(p => p.Owner)
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
