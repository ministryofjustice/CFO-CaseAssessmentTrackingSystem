using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class DocumentEntityTypeConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable(DatabaseSchema.Tables.Document);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.ContentType)
            .HasMaxLength(50)
            .IsRequired();

    }
}
