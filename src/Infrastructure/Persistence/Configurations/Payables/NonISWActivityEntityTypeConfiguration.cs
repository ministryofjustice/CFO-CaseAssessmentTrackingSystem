using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Payables;

public class NonISWActivityEntityTypeConfiguration : IEntityTypeConfiguration<NonISWActivity>
{
    public void Configure(EntityTypeBuilder<NonISWActivity> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.NonISWActivities, 
            DatabaseConstants.Schemas.Participant);
    }
}
