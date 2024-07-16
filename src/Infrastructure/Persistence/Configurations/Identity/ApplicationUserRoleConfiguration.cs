using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.UserRole, 
            DatabaseConstants.Schemas.Identity
        );

        builder
            .HasOne(d => d.Role)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(d => d.User)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.UserId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        builder.Property(x => x.RoleId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
    }
}
