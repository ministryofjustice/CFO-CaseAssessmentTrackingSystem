using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
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
        

    }
}
