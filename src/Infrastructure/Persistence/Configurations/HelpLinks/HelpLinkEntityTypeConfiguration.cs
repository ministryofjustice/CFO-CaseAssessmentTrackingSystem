using Cfo.Cats.Domain.HelpLinks;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.HelpLinks;

public class HelpLinkEntityTypeConfiguration : IEntityTypeConfiguration<HelpLink>
{
    public void Configure(EntityTypeBuilder<HelpLink> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.HelpLink, schema: DatabaseConstants.Schemas.Configuration);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(HelpLinkConstants.TitleMaximumLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(HelpLinkConstants.DescriptionMaximumLength);

        builder.OwnsMany(x => x.Urls, url =>
        {
            url.ToTable(DatabaseConstants.Tables.HelpLinkUrl, schema: DatabaseConstants.Schemas.Configuration);
            url.WithOwner().HasForeignKey("HelpLinkId");
            url.HasKey("HelpLinkId", "Id");
            url.Property<int>("Id").ValueGeneratedOnAdd();

            url.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(HelpLinkConstants.UrlMaximumLength);

            url.Property(x => x.DisplayName)
                .HasMaxLength(HelpLinkConstants.DisplayNameMaximumLength);
        });

        builder.Navigation(x => x.Urls)
            .AutoInclude();

        builder.Property(x => x.PageKey)
            .IsRequired()
            .HasMaxLength(HelpLinkConstants.PageKeyMaximumLength);

        builder.Property(x => x.TabName)
            .HasMaxLength(HelpLinkConstants.TabNameMaximumLength);

        builder.HasIndex(x => new { x.PageKey, x.TabName });

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
