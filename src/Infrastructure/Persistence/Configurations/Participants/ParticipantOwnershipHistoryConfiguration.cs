using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantOwnershipHistoryConfiguration
    : IEntityTypeConfiguration<ParticipantOwnershipHistory>
{
    public void Configure(EntityTypeBuilder<ParticipantOwnershipHistory> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.OwnershipHistory,
            DatabaseConstants.Schemas.Participant
        );

        builder.HasKey(poh => poh.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(poh => poh.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(poh => poh.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(poh => poh.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(poh => poh.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(poh => poh.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(poh => poh.ContractId)
            .IsRequired(false)
            .HasMaxLength(DatabaseConstants.FieldLengths.ContractId);
    }
}
