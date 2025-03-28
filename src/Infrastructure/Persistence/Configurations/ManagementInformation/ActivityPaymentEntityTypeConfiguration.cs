using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class ActivityPaymentEntityTypeConfiguration
    : IEntityTypeConfiguration<ActivityPayment>
{
    public void Configure(EntityTypeBuilder<ActivityPayment> builder)
    {
        builder.ToTable(nameof(ActivityPayment), DatabaseConstants.Schemas.Mi);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActivityId)
            .IsRequired();

        builder.Property(x => x.ActivityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ActivityCategory)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.ActivityApproved)
            .IsRequired()
            .HasColumnType("date");

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

        builder.Property(x => x.EligibleForPayment)
            .IsRequired();
        
        builder.Property(x => x.IneligibilityReason)
            .HasMaxLength(250);

        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.Property(x => x.CommencedDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.ActivityInput)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.PaymentPeriod)
            .IsRequired()
            .HasColumnType("date");

        builder.HasIndex(x => new { 
            x.ParticipantId,
            x.ContractId,
            x.ActivityCategory,
            x.ActivityType,
            x.ActivityApproved
        })
        .HasDatabaseName("ix_ActivityPayment_ParticipantId");

    }
}