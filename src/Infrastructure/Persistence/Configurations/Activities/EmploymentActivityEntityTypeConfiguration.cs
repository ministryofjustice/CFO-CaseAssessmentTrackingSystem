using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Activities;

public class EmploymentActivityEntityTypeConfiguration : IEntityTypeConfiguration<EmploymentActivity>
{
    public void Configure(EntityTypeBuilder<EmploymentActivity> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.EmploymentActivity,
            DatabaseConstants.Schemas.Activities);

        builder.HasOne(a => a.Document)
            .WithMany()
            .IsRequired(true);
    }
}
