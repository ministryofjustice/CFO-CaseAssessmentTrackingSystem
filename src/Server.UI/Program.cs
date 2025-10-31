using Cfo.Cats.Application;
using Cfo.Cats.Infrastructure;
using Cfo.Cats.Infrastructure.Persistence;
using Cfo.Cats.Server.UI;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-GB");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-GB");

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.AddServerUi();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, builder.Environment);

builder.AddServiceDefaults();

// Use Sentry.AspNetCore instead of Logging.AddSentry
var sentryDsn = builder.Configuration["Sentry:Dsn"];
var useSentry = !string.IsNullOrEmpty(sentryDsn);

if (useSentry)
{
    builder.WebHost.UseSentry(options =>
    {
        builder.Configuration.GetSection("Sentry").Bind(options);
        options.AddExceptionFilterForType<NavigationException>();

        options.AddEntityFramework(); 
    });
}

var app = builder.Build();

app.ConfigureServer();
app.MapDefaultEndpoints();

await app.RunAsync();
