using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DatabaseMigrator;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddRequiredServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHostedService<DatabaseMigrationService>()
            .AddDbContext<ApplicationDbContext>(dbContextOptions => {
                dbContextOptions.UseSqlServer(builder.Configuration.GetConnectionString("CatsDb")!,
                sqlServerOptionsAction: sqlServerDbContextOptionsBuilder => {
                    sqlServerDbContextOptionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
            });
        
        builder.AddServiceDefaults();
        
        return builder;
    }
}
