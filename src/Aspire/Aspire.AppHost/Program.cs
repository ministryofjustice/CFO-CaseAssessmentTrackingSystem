var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);

var sqlServer = builder.AddSqlServer("sql", password, 1433)
    .WithDataVolume("cats-aspire-data")
    .WithLifetime(ContainerLifetime.Persistent);
    

var database = sqlServer.AddDatabase("CatsDb");


builder.AddProject<Projects.Server_Ui>("cats")
    .WithReference(database)
    .WaitFor(database);


builder.Build().Run();
