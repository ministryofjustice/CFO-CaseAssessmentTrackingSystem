using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Cfo.Cats.Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .WriteTo.Sentry(options =>
            {
                options.Dsn = builder.Configuration["Sentry:Dsn"];
                options.MinimumBreadcrumbLevel = LogEventLevel.Information;
                options.MinimumEventLevel = LogEventLevel.Error;
                options.SendDefaultPii = true;
            })
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}