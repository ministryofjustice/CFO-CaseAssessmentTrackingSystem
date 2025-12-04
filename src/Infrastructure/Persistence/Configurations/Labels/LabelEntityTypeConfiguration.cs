using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Labels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cfo.Cats.Infrastructure.Constants.Database;
using Cfo.Cats.Infrastructure.Persistence.Converters;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Labels;

public class LabelEntityTypeConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Label, schema: DatabaseConstants.Schemas.Configuration);

        builder.Property(x => x.Id)
            .HasConversion(new TypedIdValueConverter<LabelId>(v => new LabelId(v)));
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasOne<Contract>()              
            .WithMany()                     
            .HasForeignKey(x => x.ContractId) 
            .IsRequired(false);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

    }
}