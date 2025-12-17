var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sqlPassword", secret: true);

var k8s = builder.AddKubernetesEnvironment("k8s");

bool publishing = builder.ExecutionContext.IsPublishMode;

#pragma warning disable ASPIREPROXYENDPOINTS001
var sql = builder.AddSqlServer("sql", sqlPassword, 1433)
    .WithDataVolume("cats-aspire-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false)
    .WithImageTag("2022-latest");
#pragma warning restore ASPIREPROXYENDPOINTS001
    
    
var catsDb = sql.AddDatabase("CatsDb");

var rabbit = builder.AddRabbitMQ("rabbit",
        port: 5672)
    .WithManagementPlugin(port: 15672);

var migrator = builder.AddProject<Projects.DatabaseMigrator>("Migrator")
    .WithReference(catsDb)
    .WaitFor(sql);

var cats = builder.AddProject<Projects.Server_UI>("cats", configure: project =>
    {
        // Exclude launchSettings on publish
        project.ExcludeLaunchProfile = publishing;
    })
    .WithReference(catsDb)
    .WithReference(rabbit);

if(builder.Configuration["MigrationBehaviour"] is "WaitForCompletion")
{
    cats.WaitForCompletion(migrator);
}
else
{
    cats.WaitFor(migrator);
}

if (publishing)
{
    cats.WithHttpEndpoint(port: 8080, targetPort: 8080)
        .WithReplicas(3);
}

builder.Build().Run();