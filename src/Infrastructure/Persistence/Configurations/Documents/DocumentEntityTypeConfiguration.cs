using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Documents;

public class DocumentEntityTypeConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Document, 
                        DatabaseConstants.Schemas.Document);
        builder.Property(t => t.DocumentType).HasConversion<string>();
        builder.Property(x => x.Content).HasMaxLength(4000);

        // In format 1.1, 1.11, 10.11
        builder.Property(x => x.Version).HasMaxLength(5);
        
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

        builder.Property(x => x.CreatedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.OwnerId).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.EditorId).HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
    }
}
