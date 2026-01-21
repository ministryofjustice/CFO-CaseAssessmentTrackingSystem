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

        builder.Property(x => x.EnrolmentHistoryId)
            .IsRequired();

        builder.Property(x => x.OccurredOn)
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Property(x => x.SupportWorker)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ContractId)
            .IsRequired()
            .HasMaxLength(12);

        builder.Property(x => x.LocationId)
            .IsRequired();

        builder.Property(x => x.LocationType)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(x => x.ArchiveReason)
            .HasMaxLength(250);

        // Helpful MI index
        builder.HasIndex(x => new
            {
                x.ParticipantId,
                x.EnrolmentHistoryId,
                x.TenantId,
                x.OccurredOn
            })
            .HasDatabaseName("ix_ArchivedCase_Participant_Enrolment");
    }
}