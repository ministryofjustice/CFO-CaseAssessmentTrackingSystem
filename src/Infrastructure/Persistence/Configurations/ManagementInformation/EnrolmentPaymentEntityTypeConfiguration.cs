using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class EnrolmentPaymentEntityTypeConfiguration : IEntityTypeConfiguration<EnrolmentPayment>
{
    public void Configure(EntityTypeBuilder<EnrolmentPayment> builder)
    {
        builder.ToTable(nameof(EnrolmentPayment), "Attachments");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.HasIndex(x => x.ParticipantId);


        builder.Property(x => x.SupportWorker)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId)
            .IsRequired();

        builder.Property(x => x.ContractId)
            .HasMaxLength(12)
            .IsRequired();
        
        builder.Property(x => x.ConsentAdded)
            .IsRequired();

        builder.Property(x => x.ConsentSigned)
            .IsRequired();

        builder.Property(x => x.SubmissionToPqa)
            .IsRequired();

        builder.Property(x => x.SubmissionToAuthority)
            .IsRequired();

        builder.Property(x => x.Approved)
            .IsRequired();

        builder.Property(x => x.LocationId)
            .IsRequired();

        builder.Property(x => x.LocationType)
            .IsRequired()
            .HasDefaultValue("")
            .HasMaxLength(25);

        builder.Property(x => x.TenantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId)
            .IsRequired();
        
        builder.Property(x => x.ReferralRoute)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.EligibleForPayment)
            .IsRequired();
        
        builder.Property(x => x.IneligibilityReason)
            .HasMaxLength(250);
    }
}


public class InductionPaymentEntityTypeConfiguration : IEntityTypeConfiguration<InductionPayment>
{
    public void Configure(EntityTypeBuilder<InductionPayment> builder)
    {
        builder.ToTable(nameof(InductionPayment), "Attachments");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.HasIndex(x => new { 
            x.ParticipantId,
            x.ContractId
        })
        .HasDatabaseName("ix_InductionPayment_ParticipantId");

        builder.Property(x => x.SupportWorker)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId)
            .IsRequired();

        builder.Property(x => x.ContractId)
            .HasMaxLength(12)
            .IsRequired();
        
        builder.Property(x => x.Approved)
            .IsRequired(false);

        builder.Property(x => x.Induction)
            .IsRequired();

        builder.Property(x => x.LocationId)
            .IsRequired();

        builder.Property(x => x.LocationType)
            .IsRequired()
            .HasDefaultValue("")
            .HasMaxLength(25);

        builder.Property(x => x.TenantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId)
            .IsRequired();
     
        builder.Property(x => x.EligibleForPayment)
            .IsRequired();
        
        builder.Property(x => x.IneligibilityReason)
            .HasMaxLength(250);
    }
}
