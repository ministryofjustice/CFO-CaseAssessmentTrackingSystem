using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Audit;

public class AccessAuditTrailEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantAccessAuditTrail>
{
    public void Configure(EntityTypeBuilder<ParticipantAccessAuditTrail> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.AccessAuditTrail, DatabaseConstants.Schemas.Audit);
        
        builder.Property<int>("Id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasKey("Id");

        builder.Property(a => a.UserId).IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(a => a.RequestType)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.ParticipantId)
            .HasMaxLength(9)
            .IsRequired();

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantId);
            
        
        builder.Property(a => a.AccessDate)
            .IsRequired();
    }
}