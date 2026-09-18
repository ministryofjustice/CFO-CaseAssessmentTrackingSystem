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
            .HasMaxLength(LabelConstants.NameMaximumLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(LabelConstants.DescriptionMaximumLength);

        builder.OwnsMany(x => x.Contracts, contract =>
        {
            contract.ToTable(DatabaseConstants.Tables.LabelContract, DatabaseConstants.Schemas.Configuration);
            contract.WithOwner().HasForeignKey("LabelId");
            contract.HasKey("LabelId", nameof(LabelContract.ContractId));
            contract.Property(x => x.ContractId)
                .HasMaxLength(DatabaseConstants.FieldLengths.ContractId);
        });

        builder.Navigation(x => x.Contracts)
            .AutoInclude();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(e => e.Scope)
            .IsRequired()
            .HasConversion(
            s => s!.Value,
            s => LabelScope.FromValue(s));

    }
}
