using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Infrastructure.Constants.Database;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.Activities;

public class EducationTrainingActivityEntityTypeConfiguration : IEntityTypeConfiguration<EducationTrainingActivity>
{
    public void Configure(EntityTypeBuilder<EducationTrainingActivity> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.EducationTrainingActivity,
            DatabaseConstants.Schemas.Activities);

        builder.Property(x => x.CourseTitle)
        .IsRequired()
        .HasMaxLength(200);

        builder.Property(x => x.CourseUrl)        
        .HasMaxLength(2000);

        builder.Property(x => x.CourseLevel)
        .IsRequired()
        .HasMaxLength(100);

        builder.HasOne(a => a.Document)
            .WithMany()
            .HasForeignKey(a => a.DocumentId)
            .IsRequired(true);

        builder.Property(a => a.CourseCompletionStatus)
            .IsRequired()
            .HasConversion(
                c => c!.Value,
                c => CourseCompletionStatus.FromValue(c)
            );
    }
}
