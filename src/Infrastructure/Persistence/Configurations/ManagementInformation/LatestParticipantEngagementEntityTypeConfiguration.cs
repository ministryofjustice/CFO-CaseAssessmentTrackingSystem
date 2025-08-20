using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class LatestParticipantEngagementEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantEngagement>
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

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Property(x => x.EngagedOn)
            .IsRequired();

        builder.HasIndex(x => new { x.EngagedOn, x.ParticipantId });
    }

}
