using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Documents;

public class GeneratedDocumentEntityTypeConfiguration : IEntityTypeConfiguration<GeneratedDocument>
{
    public void Configure(EntityTypeBuilder<GeneratedDocument> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.GeneratedDocument,
                DatabaseConstants.Schemas.Document);

        builder.Property(x => x.Template)
            .HasConversion(
                x => x!.Name,
                x => DocumentTemplate.FromName(x, false)
            )
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.ExpiresOn);

        builder.Property(d => d.ExpiresOn).IsRequired();
        builder.Property(d => d.Status).HasConversion<string>();
    }
}
