using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Notifications;

public class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
         builder.ToTable(
            DatabaseConstants.Tables.Notification,
            DatabaseConstants.Schemas.Identity
        );

        builder.HasKey(x => x.Id).IsClustered(false);

        //make the clustered index based on the created date
        builder.HasIndex(x => new { 
            x.Created
        }, "clst_notification")
        .IsClustered(true);

        builder.HasIndex(x => new {
            x.OwnerId,
            x.Created,
            x.ReadDate
        });

        builder.Property(x => x.Id)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId);

        builder.Property(x => x.OwnerId)
            .IsRequired();

        builder.Property(x => x.Heading)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Details)
            .IsRequired();

        builder.Property(x => x.Link)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.Created)
            .IsRequired();

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);
        

    }
}