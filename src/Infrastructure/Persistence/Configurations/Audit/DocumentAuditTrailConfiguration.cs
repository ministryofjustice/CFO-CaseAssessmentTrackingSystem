using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Audit;

public class DocumentAuditTrailConfiguration : IEntityTypeConfiguration<DocumentAuditTrail>
{
    public void Configure(EntityTypeBuilder<DocumentAuditTrail> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.DocumentAuditTrail, DatabaseConstants.Schemas.Audit);

        builder.HasKey(a => a.Id);
        
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(a => a.UserId);

        builder.HasOne<GeneratedDocument>()
            .WithMany()
            .HasForeignKey(a => a.DocumentId);

        builder.Property(a => a.RequestType)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.OccurredOn)
            .IsRequired();
    }
}
