using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class UsageEventEntityTypeConfiguration : IEntityTypeConfiguration<UsageEvent>
{
    public void Configure(EntityTypeBuilder<UsageEvent> builder)
    {
        builder.ToTable(nameof(UsageEvent), DatabaseConstants.Schemas.Mi);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Area)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.Fifty);

        builder.Property(x => x.Activity)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.Fifty);

        builder.Property(x => x.UserId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.UserName)
            .HasMaxLength(DatabaseConstants.FieldLengths.UserDisplayName);

        builder.Property(x => x.TenantId)
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Property(x => x.Context)
            .HasMaxLength(DatabaseConstants.FieldLengths.MediumLengthDescription);

        builder.Property(x => x.OccurredOn)
            .IsRequired();

        builder.HasIndex(x => new { x.OccurredOn, x.Area, x.Activity });
        builder.HasIndex(x => x.UserId);
    }
}
