using Cfo.Cats.Domain.Entities.Participants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Participants;

public class ParticipantContactDetailEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantContactDetail>
{
    public void Configure(EntityTypeBuilder<ParticipantContactDetail> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.PostCode)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(x => x.UPRN)
            .IsRequired()
            .HasMaxLength(12);

        builder.Property(x => x.MobileNumber)
            .HasMaxLength(16);

        builder.Property(x => x.EmailAddress)
            .HasMaxLength(256);

        builder.Property(x => x.Primary)
            .IsRequired();
    }
}
