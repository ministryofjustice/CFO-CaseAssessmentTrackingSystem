using Cfo.Cats.Application;
using Cfo.Cats.Infrastructure;
using Cfo.Cats.Infrastructure.Persistence;
using Cfo.Cats.Server;
using Cfo.Cats.Server.UI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();
builder.WebHost.UseStaticWebAssets();

builder.AddServerUi();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.ConfigureServer(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var applicationDbContextInitializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await applicationDbContextInitializer.InitialiseAsync();
}

await app.RunAsync();

Log.CloseAndFlush();