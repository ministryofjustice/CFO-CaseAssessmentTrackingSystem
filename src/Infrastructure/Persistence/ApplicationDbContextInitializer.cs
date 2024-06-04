namespace Cfo.Cats.Infrastructure.Persistence;

public class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            if (
                context.Database.IsSqlServer()
                || context.Database.IsSqlite()
            )
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }
}
