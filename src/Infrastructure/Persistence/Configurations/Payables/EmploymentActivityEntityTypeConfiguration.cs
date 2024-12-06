using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Payables;

public class EmploymentActivityEntityTypeConfiguration : IEntityTypeConfiguration<EmploymentActivity>
{
    public void Configure(EntityTypeBuilder<EmploymentActivity> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.EmploymentActivities,
            DatabaseConstants.Schemas.Participant);

        builder.HasOne(a => a.Document)
            .WithMany()
            .IsRequired(true);
    }
}
