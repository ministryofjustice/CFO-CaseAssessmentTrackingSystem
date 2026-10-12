using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Announcements;

public class AnnouncementEntityTypeConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("Announcement", DatabaseConstants.Schemas.Configuration);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreatorId).IsRequired().HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.Version).IsConcurrencyToken();
        builder.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.DeletedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Ignore(x => x.PendingRevision);
        builder.Ignore(x => x.PublishedRevision);
        builder.OwnsMany(x => x.Revisions, revision =>
        {
            revision.ToTable("AnnouncementRevision", DatabaseConstants.Schemas.Configuration);
            revision.WithOwner().HasForeignKey("AnnouncementId");
            revision.HasKey(x => x.Id);
            revision.Property(x => x.Id).ValueGeneratedNever();
            revision.Property(x => x.TypeName).IsRequired().HasMaxLength(AnnouncementConstants.TypeNameMaximumLength);
            revision.Property(x => x.Title).IsRequired().HasMaxLength(AnnouncementConstants.TitleMaximumLength);
            revision.Property(x => x.PostedBy).IsRequired().HasMaxLength(AnnouncementConstants.PostedByMaximumLength);
            revision.Property(x => x.SubmittedByName).IsRequired().HasMaxLength(256);
            revision.Property(x => x.Description).IsRequired().HasMaxLength(AnnouncementConstants.DescriptionMaximumLength);
            revision.Property(x => x.SubmittedBy).IsRequired().HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            revision.Property(x => x.ApprovedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
            revision.OwnsMany(x => x.Links, link =>
            {
                link.ToTable("AnnouncementLink", DatabaseConstants.Schemas.Configuration);
                link.WithOwner().HasForeignKey("RevisionId");
                link.Property<int>("Id").ValueGeneratedOnAdd();
                link.HasKey("RevisionId", "Id");
                link.Property(x => x.Url).IsRequired().HasMaxLength(AnnouncementConstants.UrlMaximumLength);
                link.Property(x => x.DisplayName).HasMaxLength(AnnouncementConstants.DisplayNameMaximumLength);
            });
            revision.Navigation(x => x.Links).AutoInclude();
        });
        builder.Navigation(x => x.Revisions).AutoInclude();
        builder.HasIndex(x => x.DeletedAt);
    }
}
