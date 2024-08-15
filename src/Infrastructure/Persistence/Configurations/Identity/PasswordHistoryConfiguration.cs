using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Identity;

public class PasswordHistoryConfiguration
    : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.PasswordHistory, DatabaseConstants.Schemas.Identity);
        builder.Property(ph => ph.UserId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId)
            .IsRequired();

        builder.Property(ph => ph.PasswordHash)
            .IsRequired();

        builder.Property(ph => ph.CreatedAt)
            .IsRequired();
    }
}
