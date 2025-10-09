using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Infrastructure.Persistence;

public class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context, IConfiguration configuration)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();

            if (string.IsNullOrEmpty(configuration["SeedPath"]))
            {
                throw new InvalidEnumArgumentException("Need the seed path");
            }

            DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration["SeedPath"]!));

            var conn = context.Database.GetDbConnection();

            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
            }

            var sqlFiles = di.GetFiles("*.sql").OrderBy(s => s.Name);
            foreach (var file in sqlFiles)
            {
    
                var sql = await File.ReadAllTextAsync(file.FullName);

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;

                logger.LogInformation("Executing {file}", file.FullName);
                await cmd.ExecuteNonQueryAsync();
                logger.LogInformation("Executed {file}", file.FullName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }
}