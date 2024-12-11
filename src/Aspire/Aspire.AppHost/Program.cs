var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sqlPassword", secret: true);
var rabbitUser = builder.AddParameter("rabbitUser", secret: true);
var rabbitPassword = builder.AddParameter("rabbitPassword", secret: true);

var db = builder.AddSqlServer("sql", sqlPassword, 1433)
    .WithDataVolume("cats-aspire-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("CatsDb");

var rabbit = builder.AddRabbitMQ("rabbit",
        userName: rabbitUser,
        password: rabbitPassword)
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.Server_Ui>("cats")
    .WithReference(db)
    .WithReference(rabbit)
    .WaitFor(db);

builder.Build().Run();