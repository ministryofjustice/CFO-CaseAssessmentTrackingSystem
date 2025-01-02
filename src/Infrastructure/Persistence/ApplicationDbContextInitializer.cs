using Cfo.Cats.Infrastructure.Persistence.Seeding;

namespace Cfo.Cats.Infrastructure.Persistence;

public class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
            if (await context.DateDimensions.AnyAsync() == false)
            {
                // this is not the best. But only runs in dev.
                var dateDimensions = DateDimensionSeeder.GenerateDateDimensions(
                    new DateTime(2000, 1, 1),
                    new DateTime(2050, 12, 31)
                );

                await context.DateDimensions.AddRangeAsync(dateDimensions);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }
}