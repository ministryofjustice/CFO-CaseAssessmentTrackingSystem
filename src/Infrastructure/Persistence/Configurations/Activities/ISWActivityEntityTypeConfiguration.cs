using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Activities;

public class ISWActivityEntityTypeConfiguration : IEntityTypeConfiguration<ISWActivity>
{
    public void Configure(EntityTypeBuilder<ISWActivity> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.IswActivity,
            DatabaseConstants.Schemas.Activities);

        builder.HasOne(a => a.Document)
            .WithMany()
            .IsRequired(true);
    }
}
