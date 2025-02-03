using Cfo.Cats.Infrastructure.Constants.Database;
using Cfo.Cats.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Audit;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
#nullable disable
    public void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.AuditTrail, DatabaseConstants.Schemas.Audit);

        builder
            .HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Navigation(e => e.Owner).AutoInclude();
        builder.Property(t => t.AuditType).HasConversion<string>();
        builder.Property(e => e.AffectedColumns).HasStringListConversion();
        builder.Property(u => u.OldValues).HasJsonConversion();
        builder.Property(u => u.NewValues).HasJsonConversion();
        builder.Property(u => u.PrimaryKey).HasJsonConversion();

        builder.Property(u => u.PrimaryKey)
            .HasMaxLength(150);

        builder.HasIndex(x => x.PrimaryKey);

        builder.Ignore(x => x.TemporaryProperties);
        builder.Ignore(x => x.HasTemporaryProperties);
    }
}
