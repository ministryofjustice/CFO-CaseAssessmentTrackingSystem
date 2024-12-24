using Cfo.Cats.Domain.Entities.ManagementInformation;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation
{
    public class DateDimensionEntityTypeConfiguration : IEntityTypeConfiguration<DateDimension>
    {
        public void Configure(EntityTypeBuilder<DateDimension> builder)
        {

            builder.ToTable(nameof(DateDimension));

            builder.HasKey(e => e.TheDate); 

            // Map TheDate to SQL Server DATE type
            builder.Property(e => e.TheDate).HasColumnType("date"); 
            builder.Property(e => e.TheFirstOfWeek).HasColumnType("date");
            builder.Property(e => e.TheLastOfWeek).HasColumnType("date");
            builder.Property(e => e.TheFirstOfMonth).HasColumnType("date");
            builder.Property(e => e.TheLastOfMonth).HasColumnType("date");
            builder.Property(e => e.TheFirstOfNextMonth).HasColumnType("date");
            builder.Property(e => e.TheLastOfNextMonth).HasColumnType("date");
            builder.Property(e => e.TheFirstOfQuarter).HasColumnType("date");
            builder.Property(e => e.TheLastOfQuarter).HasColumnType("date");
            builder.Property(e => e.TheFirstOfYear).HasColumnType("date");
            builder.Property(e => e.TheLastOfYear).HasColumnType("date");

            builder.Property(e => e.TheDaySuffix).HasMaxLength(2); // e.g., "st", "nd", "rd", "th"
            builder.Property(e => e.TheDayName).HasMaxLength(20); // e.g., "Monday"
            builder.Property(e => e.TheMonthName).HasMaxLength(20); // e.g., "January"
            builder.Property(e => e.MMYYYY).HasMaxLength(7); // e.g., "012023"
            builder.Property(e => e.Style101).HasMaxLength(10); // e.g., "01/23/2023"
            builder.Property(e => e.Style103).HasMaxLength(10); // e.g., "23/01/2023"
            builder.Property(e => e.Style112).HasMaxLength(8); // e.g., "20230123"
            builder.Property(e => e.Style120).HasMaxLength(10); // e.g., "2023-01-23"
            


        }
    }
}
