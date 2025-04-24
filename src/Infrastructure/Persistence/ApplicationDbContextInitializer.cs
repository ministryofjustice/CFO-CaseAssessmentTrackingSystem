namespace Cfo.Cats.Infrastructure.Persistence;

public class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();

            if (await context.Tenants.AnyAsync() == false)
            {
                logger.LogInformation("No tenant data found. Executing scripts");

                // no tenant, seed all data
                DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../db/seed"));

                var sqlFiles = di.GetFiles("*.sql").OrderBy(s => s.Name);
                foreach (var file in sqlFiles)
                {
                    logger.LogInformation("Executing {file}", file.FullName);
                    await context.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file.FullName));
                    logger.LogInformation("Executed {file}", file.FullName);
                }
            }
            else
            {
                logger.LogInformation("Tenant information found in the database. Skipping seeding");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }
}