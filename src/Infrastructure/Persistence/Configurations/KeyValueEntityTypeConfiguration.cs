using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations
{
    public class KeyValueEntityTypeConfiguration : IEntityTypeConfiguration<KeyValue>
    {
        public void Configure(EntityTypeBuilder<KeyValue> builder)
        {
            builder.ToTable(DatabaseSchema.Tables.KeyValue);
            builder.Property(t => t.Name).HasConversion<string>();
            builder.Property(t => t.Value).HasMaxLength(100).IsRequired();
            builder.Property(t => t.Text).HasMaxLength(100).IsRequired();
            builder.Property(t => t.Description).IsRequired(false);
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
