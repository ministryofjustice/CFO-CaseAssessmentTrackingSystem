using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class TimelineEntityTypeConfiguration : IEntityTypeConfiguration<Timeline>
{
    public void Configure(EntityTypeBuilder<Timeline> builder)
    {
        builder.ToTable(DatabaseSchema.Tables.Timeline);

        builder.HasKey(t => t.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey("_participantId");

        builder.Property<string>("_participantId")
            .HasColumnName("ParticipantId");

        builder.Property(t => t.EventType)
            .IsRequired()
            .HasConversion(
            tet => tet!.Value,
            tet => TimelineEventType.FromValue(tet)
            );

        builder.Property(t => t.Line1)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(t => t.Line2)
            .HasMaxLength(50);
        
        builder.Property(t => t.Line3)
            .HasMaxLength(50);

        builder.HasOne(t => t.CreatedByUser)
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        
    }
}
