using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.RoleClaim,
            DatabaseConstants.Schemas.Identity
        );

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Group)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.RoleId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(x => x.ClaimType)
            .IsRequired();

        builder.Property(x => x.ClaimValue)
            .IsRequired();
        
        builder
            .HasOne(d => d.Role)
            .WithMany(p => p.RoleClaims)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
