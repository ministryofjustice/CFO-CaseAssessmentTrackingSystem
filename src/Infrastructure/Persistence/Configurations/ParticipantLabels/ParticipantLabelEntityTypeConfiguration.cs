using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Infrastructure.Constants.Database;
using Cfo.Cats.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ParticipantLabels;

public class ParticipantLabelEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantLabel>
{
    public void Configure(EntityTypeBuilder<ParticipantLabel> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Label,
            DatabaseConstants.Schemas.Participant);
        
        builder.Property(pl => pl.Id)
            .HasConversion(new TypedIdValueConverter<ParticipantLabelId>(i => new  ParticipantLabelId(i)));

        builder.Property("LabelId")
            .HasConversion(new TypedIdValueConverter<LabelId>(i => new LabelId(i)));
        
        builder.HasKey(x => x.Id);

        builder.Property<string>("_participantId")
            .HasColumnName("ParticipantId")
            .HasMaxLength(DatabaseConstants.FieldLengths.ParticipantId)
            .IsRequired();

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey("_participantId");

        builder.HasOne(x => x.Label)
            .WithMany()
            .HasForeignKey("LabelId");
        
        builder.OwnsOne(l => l.Lifetime, lifetime => {
            lifetime.Property(l => l.StartDate).HasColumnName("LifetimeStart");
            lifetime.Property(l => l.EndDate).HasColumnName("LifetimeEnd");
        });

        builder.Property(x => x.Created)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId)
            .IsRequired();

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId)
            .IsRequired(false);
        
    }
}