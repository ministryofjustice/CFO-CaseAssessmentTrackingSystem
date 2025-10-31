using Cfo.Cats.Infrastructure.Persistence;
using DatabaseMigrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddRequiredServices();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();

var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.Database.MigrateAsync();

await app.RunAsync();
