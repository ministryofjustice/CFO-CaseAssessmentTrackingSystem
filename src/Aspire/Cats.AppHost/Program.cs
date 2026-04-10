using Cats.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sqlPassword", secret: true);

var sql = builder.AddCatsSqlServer(sqlPassword);

var databases = builder.AddCatsDatabases(sql, seedData: true);

var rabbit = builder.AddRabbitMQ("rabbit",
        port: 5672)
    .WithDataVolume("cats-aspire-rabbit")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin(port: 15672);

builder.AddCatsServices(
    rabbit, 
    databases);

builder.Build().Run();