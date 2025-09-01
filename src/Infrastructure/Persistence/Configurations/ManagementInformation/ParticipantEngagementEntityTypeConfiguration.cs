using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class ParticipantEngagementEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantEngagement>
{
    public void Configure(EntityTypeBuilder<ParticipantEngagement> builder)
    {
        builder.ToTable(nameof(ParticipantEngagement), DatabaseConstants.Schemas.Mi);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ParticipantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.EngagedWith)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.UserDisplayName);

        builder.Property(x => x.EngagedWithTenant)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantName);

        builder.Property(x => x.EngagedOn)
            .IsRequired();

        builder.Property(x => x.EngagedAtLocation)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.LocationName);

        builder.Property(x => x.EngagedAtContract)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.ContractDescription);

        builder.HasIndex(x => new { x.EngagedOn, x.ParticipantId });
    }

}
