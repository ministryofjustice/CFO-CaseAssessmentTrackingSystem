using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Payables;

public class ISWActivityEntityTypeConfiguration : IEntityTypeConfiguration<ISWActivity>
{
    public void Configure(EntityTypeBuilder<ISWActivity> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.ISWActivities,
            DatabaseConstants.Schemas.Payables);

        builder.HasOne(a => a.Document)
            .WithMany()
            .IsRequired(true);
    }
}
