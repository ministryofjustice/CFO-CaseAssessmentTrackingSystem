using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.UserLogin,
            DatabaseConstants.Schemas.Identity);

        builder.Property(x => x.UserId)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder
            .HasOne(d => d.User)
            .WithMany(p => p.Logins)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
