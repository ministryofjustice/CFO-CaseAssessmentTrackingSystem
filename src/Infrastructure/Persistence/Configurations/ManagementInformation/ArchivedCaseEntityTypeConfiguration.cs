using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class ArchivedCaseEntityTypeConfiguration
    : IEntityTypeConfiguration<ArchivedCase>
{
    public void Configure(EntityTypeBuilder<ArchivedCase> builder)
    {
        builder.ToTable(nameof(ArchivedCase), DatabaseConstants.Schemas.Mi);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);

      builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.EnrolmentHistoryId)
            .IsRequired();

        builder.Property(x => x.Created)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.AdditionalInfo)
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(x => x.ArchiveReason)
            .HasMaxLength(250);

        builder.Property(x => x.From)
            .IsRequired();
        
        builder.Property(x => x.To);
        
        builder.Property(x => x.ContractId)
            .IsRequired()
            .HasMaxLength(12);

        builder.Property(x => x.LocationId)
            .IsRequired();

        builder.Property(x => x.LocationType)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);
        
        builder.HasIndex(x => new
            {
                x.ParticipantId,
                x.EnrolmentHistoryId,
                x.TenantId,
                x.From
            })
            .HasDatabaseName("ix_ArchivedCase_Participant_Enrolment");
    }
}