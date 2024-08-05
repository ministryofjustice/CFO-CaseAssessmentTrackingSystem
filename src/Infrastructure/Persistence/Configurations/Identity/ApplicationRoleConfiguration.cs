using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.Role,
            DatabaseConstants.Schemas.Identity
            );
        
        builder.Property(x => x.Id)
            .HasMaxLength(36);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);        
        
        builder.Property(x => x.NormalizedName)
              .IsRequired()
              .HasMaxLength(50);

        builder.Property(x => x.ConcurrencyStamp)
            .IsRequired()
            .HasMaxLength(36);
    }
}
