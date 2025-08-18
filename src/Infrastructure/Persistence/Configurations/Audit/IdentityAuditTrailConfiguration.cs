using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Audit;

public class IdentityAuditTrailConfiguration : IEntityTypeConfiguration<IdentityAuditTrail>
{
    public void Configure(EntityTypeBuilder<IdentityAuditTrail> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.IdentityAuditTrail, DatabaseConstants.Schemas.Audit);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActionType).HasConversion<string>();
        builder.Property(x => x.ActionType)
            .HasMaxLength(30)            
            .IsRequired();
        builder.Property(x => x.UserName)
            .HasMaxLength(100);
        builder.Property(x => x.PerformedBy)
            .HasMaxLength(100);
        builder.Property(x => x.DateTime)
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasMaxLength(30);

        builder.HasIndex(x => new
            {
                x.UserName,
                x.DateTime
            })
            .HasDatabaseName("idx_IdentityAudit_UserName_DateTime");
    }
}