using Cats.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sqlPassword", secret: true);

var sql = builder.AddCatsSqlServer(sqlPassword);

var databases = builder.AddCatsDatabases(sql);

var rabbit = builder.AddMessageBroker();

builder.AddCatsServices(
    rabbit, 
    databases);

builder.Build().Run();