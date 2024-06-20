using Cfo.Cats.Application;
using Cfo.Cats.Infrastructure;
using Cfo.Cats.Infrastructure.Persistence;
using Cfo.Cats.Server;
using Cfo.Cats.Server.UI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();
builder.WebHost.UseStaticWebAssets();

builder
    .Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddServerUi(builder.Configuration);

var app = builder.Build();

app.ConfigureServer(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await initializer.InitialiseAsync();
}

await app.RunAsync();

Log.CloseAndFlush();