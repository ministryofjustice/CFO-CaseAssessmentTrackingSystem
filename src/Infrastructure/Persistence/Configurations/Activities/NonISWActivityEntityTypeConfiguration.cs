using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Activities;

public class NonISWActivityEntityTypeConfiguration : IEntityTypeConfiguration<NonISWActivity>
{
    public void Configure(EntityTypeBuilder<NonISWActivity> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.NonIsqActivity, 
            DatabaseConstants.Schemas.Activities);
    }
}
