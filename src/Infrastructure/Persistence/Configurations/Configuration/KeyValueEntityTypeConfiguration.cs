using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Configuration
{
    public class KeyValueEntityTypeConfiguration : IEntityTypeConfiguration<KeyValue>
    {
        public void Configure(EntityTypeBuilder<KeyValue> builder)
        {
            builder.ToTable(DatabaseConstants.Tables.KeyValue, 
                DatabaseConstants.Schemas.Configuration);
            builder.Property(t => t.Name).HasConversion<string>();
            builder.Property(t => t.Name).HasMaxLength(50);
            builder.Property(t => t.Value).HasMaxLength(100).IsRequired();
            builder.Property(t => t.Text).HasMaxLength(100).IsRequired();
            builder.Property(t => t.Description).HasMaxLength(100).IsRequired(false);
            builder.Ignore(e => e.DomainEvents);
            
            builder.Property(x => x.CreatedBy)
                .HasMaxLength(36);
        
            builder.Property(x => x.LastModifiedBy)
                .HasMaxLength(36);
            
        }
    }
}
