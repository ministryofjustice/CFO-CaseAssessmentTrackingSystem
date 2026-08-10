using Cfo.Cats.Application;
using Cfo.Cats.Infrastructure;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-GB");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-GB");

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddConsumerInfrastructure(builder.Configuration, builder.Environment);

builder.AddServiceDefaults();

// Use Sentry.AspNetCore instead of Logging.AddSentry.
// Without this, message consumer failures (which log via ILogger) never reach Sentry.
var sentryDsn = builder.Configuration["Sentry:Dsn"];
var useSentry = !string.IsNullOrEmpty(sentryDsn);

if (useSentry)
{
    builder.WebHost.UseSentry(options =>
    {
        builder.Configuration.GetSection("Sentry").Bind(options);

        options.AddEntityFramework();
    });
}

var app = builder.Build();

app.UseRequestTimeouts();
app.UseOutputCache();

app.MapDefaultEndpoints();

await app.RunAsync();
