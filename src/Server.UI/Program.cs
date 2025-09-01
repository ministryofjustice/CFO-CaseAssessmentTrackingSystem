using Cfo.Cats.Application;
using Cfo.Cats.Infrastructure;
using Cfo.Cats.Infrastructure.Persistence;
using Cfo.Cats.Server;
using Cfo.Cats.Server.UI;

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

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var applicationDbContextInitializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await applicationDbContextInitializer.InitialiseAsync();
}

app.MapDefaultEndpoints();

await app.RunAsync();
