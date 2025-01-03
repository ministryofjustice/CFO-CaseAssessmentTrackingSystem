var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sqlPassword", secret: true);
var rabbitUser = builder.AddParameter("rabbitUser", secret: true);
var rabbitPassword = builder.AddParameter("rabbitPassword", secret: true);

var sql = builder.AddSqlServer("sql", sqlPassword, 1433)
    .WithDataVolume("cats-aspire-data")
    .WithLifetime(ContainerLifetime.Persistent);
    
    
var catsDb = sql.AddDatabase("CatsDb");

var rabbit = builder.AddRabbitMQ("rabbit",
        userName: rabbitUser,
        password: rabbitPassword)
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.Server_UI>("cats")
    .WithReference(catsDb)
    .WithReference(rabbit)
    .WaitFor(sql);

builder.Build().Run();