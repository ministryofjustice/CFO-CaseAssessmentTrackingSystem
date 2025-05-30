using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.User, 
            DatabaseConstants.Schemas.Identity
            );

        builder.Property(x => x.Id)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProviderId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);
        
        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Property(x => x.ProviderId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

        builder.Property(x => x.TenantName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.MemorablePlace)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.MemorableDate)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        
        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);
        
        
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
            note.WithOwner().HasForeignKey("UserId");

            note.Property("UserId")
                .HasMaxLength(36);
            
            note.HasKey("Id");
            note.ToTable(
                DatabaseConstants.Tables.Note,
                DatabaseConstants.Schemas.Identity);
            note.Property(x => x.Message).HasMaxLength(255);
            
            note.Property(x => x.CallReference).HasMaxLength(DatabaseConstants.FieldLengths.CallReference);

            note.Property(x => x.TenantId)
                .IsRequired()
                .HasMaxLength(DatabaseConstants.FieldLengths.TenantId);

            note.HasOne(n => n.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy);
            
            note.Property(n => n.CreatedBy)
                .HasMaxLength(36);

            note.HasOne(n => n.LastModifiedByUser)
                .WithMany()
                .HasForeignKey(n => n.LastModifiedBy);
            
            note.Property(n => n.LastModifiedBy)
                .HasMaxLength(36);
            
        });
    }
}
