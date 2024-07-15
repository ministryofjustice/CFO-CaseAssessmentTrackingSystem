using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable(DatabaseSchema.Tables.ApplicationUser);

        // Each User can have many UserLogins
        builder.HasMany(e => e.Logins).WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

        // Each User can have many UserTokens
        builder.HasMany(e => e.Tokens).WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany(e => e.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired();

        builder.HasOne(x => x.Superior).WithMany().HasForeignKey(u => u.SuperiorId);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(u => u.TenantId);
        builder.Navigation(e => e.Tenant).AutoInclude();

        builder.OwnsMany(x => x.Notes, note => {
            note.WithOwner().HasForeignKey("ApplicationUserId");
            note.HasKey("Id");
            note.ToTable(DatabaseSchema.Tables.ApplicationUserNote);
            note.Property(x => x.Message).HasMaxLength(255);
            note.HasOne(n => n.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy);
            //note.Property(n => n.CreatedBy)
            //    .HasMaxLength(36);
        });
    }
}
