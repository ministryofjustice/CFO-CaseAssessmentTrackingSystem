var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sqlPassword", secret: true);
var rabbitUser = builder.AddParameter("rabbitUser", secret: true);
var rabbitPassword = builder.AddParameter("rabbitPassword", secret: true);

#pragma warning disable ASPIREPROXYENDPOINTS001
var sql = builder.AddSqlServer("sql", sqlPassword, 1433)
    .WithDataVolume("cats-aspire-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false);
#pragma warning restore ASPIREPROXYENDPOINTS001
    
    
var catsDb = sql.AddDatabase("CatsDb");

var rabbit = builder.AddRabbitMQ("rabbit",
        userName: rabbitUser,
        password: rabbitPassword,
        port: 5672)
    .WithDataVolume("cats-aspire-rabbit")
    .WithManagementPlugin(port: 15672)
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.Server_UI>("cats")
    .WithReference(catsDb)
    .WithReference(rabbit)
    .WaitFor(sql);

builder.Build().Run();