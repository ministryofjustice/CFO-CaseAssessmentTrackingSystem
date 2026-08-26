using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

public class ContractTargetEntityTypeConfiguration : IEntityTypeConfiguration<ContractTarget>
{
    public void Configure(EntityTypeBuilder<ContractTarget> builder)
    {
        builder.ToTable(nameof(ContractTarget), DatabaseConstants.Schemas.Mi);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContractId)
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Month)
            .IsRequired();

        builder.HasIndex(x => new { x.ContractId, x.Year, x.Month })
            .IsUnique();
    }
}
