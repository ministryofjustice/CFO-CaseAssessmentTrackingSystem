using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class DocumentEntityTypeConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable(nameof(Document));
        builder.Property(t => t.DocumentType).HasConversion<string>();
        builder.Property(x => x.Content).HasMaxLength(4000);
        builder.Ignore(e => e.DomainEvents);
        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Editor)
            .WithMany()
            .HasForeignKey(x => x.LastModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(e => e.Owner).AutoInclude();
        builder.Navigation(e => e.Editor).AutoInclude();
    }
}
