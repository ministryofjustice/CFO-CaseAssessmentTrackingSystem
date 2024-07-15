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
      
        builder.Navigation(e => e.Owner).AutoInclude();
        
        builder.Property(c => c.CreatedBy)
            .HasMaxLength(36);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(36);
        
    }
}
