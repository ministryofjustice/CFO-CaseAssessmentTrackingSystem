using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Announcements;

public class AnnouncementTypeEntityTypeConfiguration : IEntityTypeConfiguration<AnnouncementType>
{
    public void Configure(EntityTypeBuilder<AnnouncementType> builder)
    {
        builder.ToTable("AnnouncementType", DatabaseConstants.Schemas.Configuration);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(AnnouncementConstants.TypeNameMaximumLength)
            .IsConcurrencyToken();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
