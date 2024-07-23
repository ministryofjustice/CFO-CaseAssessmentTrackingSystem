using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Cfo.Cats.Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}