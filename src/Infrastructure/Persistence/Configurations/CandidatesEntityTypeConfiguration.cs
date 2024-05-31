using Cfo.Cats.Domain.Entities.Candidates;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations;

public class CandidatesEntityTypeConfiguration : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder.ToTable(DatabaseSchema.Tables.Candidate);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasMaxLength(9)
            .ValueGeneratedNever();

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.MiddleName)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.HasOne(p => p.CurrentLocation)
            .WithMany()
            .HasForeignKey(p => p.CurrentLocationId);

        builder.OwnsMany(c => c.Identifiers, a => {
            a.WithOwner()
                .HasForeignKey("CandidateId");

            a.ToTable(DatabaseSchema.Tables.CandidateIdentifier);
            
            a.Property<int>("Id");
            a.HasKey("Id");

            a.Property(i => i.IdentifierValue)
                .IsRequired()
                .HasMaxLength(20);

            a.Property(i => i.IdentifierType)
                .IsRequired()
                .HasMaxLength(20);


        });
    }


}
